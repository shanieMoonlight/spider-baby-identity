using ClArch.ValueObjects;
using ID.Application.AppAbs.ApplicationServices.User;
using ID.Application.Customers.Abstractions;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.AppUsers.OAuth;
using ID.Domain.Entities.AppUsers.ValueObjects;
using ID.OAuth.Amazon.Data;
using ID.OAuth.Amazon.Features.SignIn;
using ID.OAuth.Amazon.Services.Abs;
using MyResults;

namespace ID.OAuth.Amazon.Services.Imps;

internal class FindOrCreateService<TUser>(
    IFindUserService<TUser> _findUserService,
    IIdCustomerRegistrationService _signupService)
    : IFindOrCreateService<TUser> where TUser : AppUser
{
    public async Task<GenResult<AppUser>> FindOrCreateUserAsync(
      AmazonUserProfile userProfile,
      AmazonSignInDto dto,
      CancellationToken cancellationToken)
    {
        var user = await _findUserService.FindUserWithTeamDetailsAsync(email: userProfile.Email);
        if (user != null)
            return GenResult<AppUser>.Success(user);

        var email = string.IsNullOrWhiteSpace(userProfile.Email)
            ? dto.Email
            : userProfile.Email;

        if (string.IsNullOrWhiteSpace(email))
            return GenResult<AppUser>.Failure("An Email is required for account registration");

        // Until we confirm Amazon email verification semantics, mark as unverified
        OAuthInfo oAuth = OAuthInfo.Create(
            OAuthProvider.Amazon,
            IssuerNullable.Create("Amazon"),
            ImgUrlNullable.Create(null),
            EmailVerifiedNullable.Create(false));

        return await _signupService.RegisterOAuthAsync(
                    EmailAddress.Create(email),
                    UsernameNullable.Create(userProfile.Email),
                    PhoneNullable.Create(null),
                    FirstNameNullable.Create(userProfile.Name),
                    LastNameNullable.Create(null),
                    TeamPositionNullable.Create(),
                    oAuth,
                    dto.SubscriptionPlanId,
                    cancellationToken);
    }
}
