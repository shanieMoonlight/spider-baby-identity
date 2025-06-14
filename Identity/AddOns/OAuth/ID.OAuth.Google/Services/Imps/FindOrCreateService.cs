using ClArch.ValueObjects;
using ID.Application.AppAbs.ApplicationServices.User;
using ID.Application.Customers.Abstractions;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.AppUsers.OAuth;
using ID.Domain.Entities.AppUsers.ValueObjects;
using ID.OAuth.Google.Data;
using ID.OAuth.Google.Features.SignIn;
using ID.OAuth.Google.Services.Abs;
using MyResults;

namespace ID.OAuth.Google.Services.Imps;
internal class FindOrCreateService<TUser>(
    IFindUserService<TUser> _findUserService, 
    IIdCustomerRegistrationService _signupService) 
    : IFindOrCreateService<TUser> where TUser : AppUser
{

    public async Task<GenResult<AppUser>> FindOrCreateUserAsync(
      GoogleVerifiedPayload payload,
      GoogleSignInDto dto,
      CancellationToken cancellationToken)
    {
        var user = await _findUserService.FindUserWithTeamDetailsAsync(email: payload.Email);

        if (user != null)
            return GenResult<AppUser>.Success(user);

        OAuthInfo oAuth = OAuthInfo.Create(
            OAuthProvider.Google,
            IssuerNullable.Create(payload.Issuer),
            ImgUrlNullable.Create(payload.Picture),
            EmailVerifiedNullable.Create(payload.EmailVerified));



        return await _signupService.RegisterOAuthAsync(
                    EmailAddress.Create(payload.Email),
                    UsernameNullable.Create(payload.Email),
                    PhoneNullable.Create(null),
                    FirstNameNullable.Create(payload.GivenName),
                    LastNameNullable.Create(payload.FamilyName),
                    TeamPositionNullable.Create(),
                    oAuth,
                    dto.SubscriptionId,
                    cancellationToken);
    }

}
