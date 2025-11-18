using ID.Application.AppAbs.ApplicationServices.TwoFactor;
using ID.Application.AppAbs.SignIn;
using ID.Application.AppAbs.TokenVerificationServices;
using ID.Application.Dtos.Account.Cookies;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Domain.Utility.Messages;
using ID.OAuth.Facebook.Services.Abs;
using MyResults;

namespace ID.OAuth.Facebook.Features.SignIn.FacebookCookieSignIn;

public class FacebookCookieSignInCmdHandler(
    IFindOrCreateService<AppUser> _findOrCreate,
    ICookieAuthService<AppUser> _cookieSignInService,
    IFacebookAuthenticationService _verifier,
    ITwoFactorVerificationService<AppUser> _2FactorService,
    ITwoFactorMsgService _twoFactorMsgService)
    : IIdCommandHandler<FacebookCookieSignInCmd, CookieSignInResultData>
{

    public async Task<GenResult<CookieSignInResultData>> Handle(FacebookCookieSignInCmd request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        // Combined verification + profile fetch
        var profileResult = await _verifier.VerifyAndGetProfileAsync(dto.AuthToken, dto.Id, cancellationToken);
        if (!profileResult.Succeeded)
            return profileResult.Convert<CookieSignInResultData>();

        var userProfile = profileResult.Value!; // non-null on success


        var userResult = await _findOrCreate.FindOrCreateUserAsync(userProfile, dto, cancellationToken);
        if (!userResult.Succeeded)
            return userResult.Convert<CookieSignInResultData>();

        AppUser user = userResult.Value!;  //Success is non-null
        Team team = user.Team!;


        var twoFactorEnabled = await _2FactorService.IsTwoFactorEnabledAsync(user);

        return twoFactorEnabled
            ? await ReturnTwoFactorCookieAsync(
               user: user,
               team: team,
               dto.RememberMe,
               currentDeviceId: dto.DeviceId)
            : await ReturnStandardCookieAsync(
                 user: user,
                 team: team,
                 dto.RememberMe,
                 currentDeviceId: dto.DeviceId);


    }

    //-----------------------------//

    private async Task<GenResult<CookieSignInResultData>> ReturnStandardCookieAsync(
        AppUser user,
        Team team,
        bool rememberMe,
        string? currentDeviceId)
    {
        await _cookieSignInService.SignInAsync(
                isPersistent: rememberMe,
                user: user!,
                team: team!,
                currentDeviceId: currentDeviceId);

        return GenResult<CookieSignInResultData>.Success(CookieSignInResultData.Success());

    }


    //- - - - - - - - - - - - - - -//

    private async Task<GenResult<CookieSignInResultData>> ReturnTwoFactorCookieAsync(
       AppUser user,
       Team team,
       bool rememberMe,
       string? currentDeviceId)
    {
        var twoFactorResult = await _twoFactorMsgService.SendOTPFor2FactorAuth(team, user);
        if (!twoFactorResult.Succeeded)
            return GenResult<CookieSignInResultData>.Failure(twoFactorResult.Info);

        MfaResultData mfaResultData = twoFactorResult.Value!; //Success is non-null

        await _cookieSignInService.CreateWithTwoFactorRequiredAsync(
                isPersistent: rememberMe,
                user: user!,
                currentDeviceId);

        var data = CookieSignInResultData.CreateWithTwoFactoRequired(
            provider: mfaResultData.TwoFactorProvider,
            message: IDMsgs.Error.Authorization.TWO_FACTOR_REQUIRED(mfaResultData.TwoFactorProvider));

        return GenResult<CookieSignInResultData>.PreconditionRequiredResult(data);

    }

}//Cls

