using ID.Application.AppAbs.ApplicationServices.TwoFactor;
using ID.Application.AppAbs.SignIn;
using ID.Application.AppAbs.TokenVerificationServices;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Domain.Utility.Messages;
using ID.OAuth.Google.Services.Abs;
using MyResults;

namespace ID.OAuth.Google.Features.SignIn.GoogleCookieSignIn;

public class GoogleCookieSignInCmdHandler(
    IFindOrCreateService<AppUser> _findOrCreate,
    ICookieSignInService<AppUser> _cookieSignInService,
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


        var tfEnabled = await _2FactorService.IsTwoFactorEnabledAsync(user);

        if (tfEnabled)
        {
            var twoFactorResult = await SendTwoFactoAndAttachCookieAsync(
                   user: user,
                   team: user.Team!,
                   dto.RememberMe,
                   currentDeviceId: dto.DeviceId);

            if (!twoFactorResult.Succeeded)
                return twoFactorResult.Convert<CookieSignInResultData>();

            var mfaResultData = twoFactorResult.Value!; //Success is non-null
            var data = CookieSignInResultData.CreateWithTwoFactoRequired(
                provider: mfaResultData.TwoFactorProvider,
                message: IDMsgs.Error.Authorization.TWO_FACTOR_REQUIRED(mfaResultData.TwoFactorProvider));

            return GenResult<CookieSignInResultData>.PreconditionRequiredResult(data); 
        }
        else
        {
            await AttachCookieAsync(
                 user: user,
                 team: user.Team!,
                 dto.RememberMe,
                 currentDeviceId: dto.DeviceId);
        }

        return GenResult<CookieSignInResultData>.Success(CookieSignInResultData.Success());


    }

    //-----------------------------//

    private async Task AttachCookieAsync(
        AppUser user,
        Team team,
        bool rememberMe,
        string? currentDeviceId = null)
    {
        await _cookieSignInService.SignInWithTwoFactorRequiredAsync(
                isPersistent: rememberMe,
                user: user!,
                team: team!,
                currentDeviceId: currentDeviceId);

    }


    //- - - - - - - - - - - - - - -//


    private async Task<GenResult<MfaResultData>> SendTwoFactoAndAttachCookieAsync(
        AppUser user,
        Team team,
        bool rememberMe,
        string? currentDeviceId = null)
    {
        var tfResultMsg = await _twoFactorMsgService.SendOTPFor2FactorAuth(team, user);
        if (!tfResultMsg.Succeeded)
            return GenResult<MfaResultData>.Failure(tfResultMsg.Info);

        MfaResultData mfaResultData = tfResultMsg.Value!; //Success is non-null

        await _cookieSignInService.SignInAsync(
                isPersistent: rememberMe,
                user: user!,
                team: team!,
                false,
                currentDeviceId: currentDeviceId);

        return GenResult<MfaResultData>.Success(mfaResultData);
    }


    //- - - - - - - - - - - - - - -//
}//Cls

