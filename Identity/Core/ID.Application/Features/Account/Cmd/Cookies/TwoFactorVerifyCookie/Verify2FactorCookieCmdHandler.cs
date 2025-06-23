using ID.Application.AppAbs.ApplicationServices.User;
using ID.Application.AppAbs.SignIn;
using ID.Application.AppAbs.TokenVerificationServices;
using ID.Application.Mediatr.CqrsAbs;
using ID.Application.MFA;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Utility.Messages;
using MyResults;

namespace ID.Application.Features.Account.Cmd.Cookies.TwoFactorVerifyCookie;
public class Verify2FactorCookieCmdHandler(
    ICookieAuthService<AppUser> _cookieSignInService,
    IFindUserService<AppUser> _findUserService,
    ITwofactorUserIdCacheService _twofactorUserIdCache,
    ITwoFactorVerificationService<AppUser> _2FactorService)
    : IIdCommandHandler<Verify2FactorCookieCmd>
{

    public async Task<BasicResult> Handle(Verify2FactorCookieCmd request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var token = _cookieSignInService.TryGetTwoFactorToken();
        var deviceId = _cookieSignInService.TryGetDeviceId();
        var rememberMe = _cookieSignInService.GetRememberMe();

        if (string.IsNullOrWhiteSpace(token))
            return BasicResult.BadRequestResult(IDMsgs.Error.TwoFactor.INVALID_2_FACTOR_CODE);

        var userId = _twofactorUserIdCache.GetUserId(token);
        var user = await _findUserService.FindUserWithTeamDetailsAsync(userId: userId);
        var team = user?.Team;


        if (user is null || team is null)
            return BasicResult.BadRequestResult(IDMsgs.Error.TwoFactor.INVALID_2_FACTOR_CODE);


        bool validVerification = await _2FactorService.VerifyTwoFactorTokenAsync(team, user, dto.Code);
        if (!validVerification)
            return BasicResult.BadRequestResult(IDMsgs.Error.TwoFactor.INVALID_2_FACTOR_CODE);

            //Attach cookie
            await _cookieSignInService.SignInAsync(
           rememberMe,
           user!,
           team!,
           false,
           deviceId);

        return BasicResult.Success("Signed In!");

    }


}//Cls
