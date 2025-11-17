using ClArch.ValueObjects;
using ID.Application.AppAbs.ApplicationServices.User;
using ID.Application.Customers.Abstractions;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.AppUsers.OAuth;
using ID.Domain.Entities.AppUsers.ValueObjects;
using ID.OAuth.Facebook.Data;
using ID.OAuth.Facebook.Features.SignIn;
using MyResults;

namespace ID.OAuth.Facebook.Services;

internal class FindOrCreateService<TUser>(
    IFindUserService<TUser> _findUserService,
    IIdCustomerRegistrationService _signupService)
    : IFindOrCreateService<TUser> where TUser : AppUser
{

    public async Task<GenResult<AppUser>> FindOrCreateUserAsync(
      FacebookUserProfile userProfile,
      FacebookSignInDto dto,
      CancellationToken cancellationToken)
    {
        var user = await _findUserService.FindUserWithTeamDetailsAsync(email: userProfile.Email);

        if (user != null)
            return GenResult<AppUser>.Success(user);


        if (user != null)
            return GenResult<AppUser>.Success(user);


        var email = string.IsNullOrWhiteSpace(userProfile.Email)
            ? dto.Email
            : userProfile.Email;

        if (string.IsNullOrWhiteSpace(email))
            return GenResult<AppUser>.Failure("An Email is required for account registration");


        OAuthInfo oAuth = OAuthInfo.Create(
            OAuthProvider.Facebook,
            IssuerNullable.Create("Facebook"),
            ImgUrlNullable.Create(userProfile.Picture?.Data?.Url),
            EmailVerifiedNullable.Create(false));



        return await _signupService.RegisterOAuthAsync(
                    EmailAddress.Create(email),
                    UsernameNullable.Create(userProfile.Email),
                    PhoneNullable.Create(null),
                    FirstNameNullable.Create(userProfile.FirstName),
                    LastNameNullable.Create(userProfile.LastName),
                    TeamPositionNullable.Create(),
                    oAuth,
                    dto.SubscriptionPlanId,
                    cancellationToken);
    }

}//Cls  
