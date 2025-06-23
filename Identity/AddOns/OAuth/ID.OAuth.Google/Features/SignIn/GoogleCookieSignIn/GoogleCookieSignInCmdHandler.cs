using ID.Application.AppAbs.ApplicationServices.TwoFactor;
using ID.Application.AppAbs.SignIn;
using ID.Application.AppAbs.TokenVerificationServices;
using ID.Application.Dtos.Account.Cookies;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Domain.Utility.Messages;
using ID.OAuth.Google.Services.Abs;
using MyResults;

namespace ID.OAuth.Google.Features.SignIn.GoogleCookieSignIn;

public class GoogleCookieSignInCmdHandler(
    IFindOrCreateService<AppUser> _findOrCreate,
    ICookieAuthService<AppUser> _cookieSignInService,
    IGoogleTokenVerifier _verifier,
    ITwoFactorVerificationService<AppUser> _2FactorService,
    ITwoFactorMsgService _twoFactorMsgService)
    : IIdCommandHandler<GoogleCookieSignInCmd, CookieSignInResultData>
{

    public async Task<GenResult<CookieSignInResultData>> Handle(GoogleCookieSignInCmd request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;


        var verifyResult = await _verifier.VerifyTokenAsync(dto.IdToken, cancellationToken);
        if (!verifyResult.Succeeded)
            return verifyResult.Convert<CookieSignInResultData>();


        var payload = verifyResult.Value!; //Success is non-null


        var userResult = await _findOrCreate.FindOrCreateUserAsync(payload, dto, cancellationToken);
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
                false,
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

