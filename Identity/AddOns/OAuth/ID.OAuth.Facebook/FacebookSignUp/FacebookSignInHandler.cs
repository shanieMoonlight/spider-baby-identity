using ClArch.ValueObjects;
using ID.Application.AppAbs.ApplicationServices.User;
using ID.Application.Customers.Abstractions;
using ID.Application.JWT;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.AppUsers.OAuth;
using ID.Domain.Entities.AppUsers.ValueObjects;
using ID.Domain.Models;
using ID.OAuth.Facebook.Data;
using ID.OAuth.Facebook.Services;
using MyResults;

namespace ID.OAuth.Facebook.FacebookSignUp;

/// <summary>
/// Handler for Facebook OAuth sign-in operations.
/// Implements secure server-side token verification and user creation/authentication.
/// </summary>
public class FacebookSignInHandler(
    IFindUserService<AppUser> findUserService,
    IJwtPackageProvider jwtPackageProvider,
    IFacebookTokenVerifier verifier,
    IIdCustomerRegistrationService signupService)
    : IIdCommandHandler<FacebookSignInCmd, JwtPackage>
{
    public async Task<GenResult<JwtPackage>> Handle(FacebookSignInCmd request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        // Step 1: Verify Facebook access token server-side
        var verifyResult = await verifier.VerifyTokenAsync(dto.FacebookAccessToken, cancellationToken);
        if (!verifyResult.Succeeded)
            return verifyResult.Convert<JwtPackage>();

        var payload = verifyResult.Value!;

        // Step 2: Find existing user or create new user using ONLY verified data
        var userResult = await FindOrCreateUserAsync(payload, dto, cancellationToken);
        if (!userResult.Succeeded)
            return userResult.Convert<JwtPackage>();

        var user = userResult.Value!;

        // Step 3: Create JWT package with appropriate 2FA settings
        var jwtPackage = await jwtPackageProvider.CreateJwtPackageAsync(
            user: user,
            team: user.Team!,
            currentDeviceId: dto.DeviceId,
            cancellationToken: cancellationToken);

        return GenResult<JwtPackage>.Success(jwtPackage);
    }

    //----------------------//

    /// <summary>
    /// Finds an existing user by email or creates a new one using verified Facebook data.
    /// All identity information comes from Facebook's verified API response.
    /// </summary>
    private async Task<GenResult<AppUser>> FindOrCreateUserAsync(
        FacebookVerifiedPayload payload,
        FacebookSignInDto dto,
        CancellationToken cancellationToken)
    {
        // Try to find existing user by email
        var existingUser = await findUserService.FindUserWithTeamDetailsAsync(
            email: payload.Email);

        if (existingUser != null)
        {
            return GenResult<AppUser>.Success(existingUser);
        }

        // Create new user with verified Facebook data
        var oAuth = OAuthInfo.Create(
            OAuthProvider.Facebook,
            IssuerNullable.Create("facebook.com"),
            ImgUrlNullable.Create(payload.Picture),
            EmailVerifiedNullable.Create(payload.EmailVerified));

        return await signupService.RegisterOAuthAsync(
            EmailAddress.Create(payload.Email),
            UsernameNullable.Create(payload.Email), // Use email as username
            PhoneNullable.Create(null), // Facebook doesn't typically provide phone
            FirstNameNullable.Create(payload.FirstName),
            LastNameNullable.Create(payload.LastName),
            TeamPositionNullable.Create(), // No position from Facebook
            oAuth,
            dto.SubscriptionId, // Business subscription (trusted from your system)
            cancellationToken);
    }

}//Cls
