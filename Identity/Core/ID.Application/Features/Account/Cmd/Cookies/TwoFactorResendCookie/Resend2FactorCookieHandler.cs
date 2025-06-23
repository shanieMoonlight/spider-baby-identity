using ID.Application.AppAbs.ApplicationServices.TwoFactor;
using ID.Application.AppAbs.ApplicationServices.User;
using ID.Application.AppAbs.SignIn;
using ID.Application.AppAbs.TokenVerificationServices;
using ID.Application.Mediatr.CqrsAbs;
using ID.Application.MFA;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Models;
using ID.Domain.Utility.Messages;
using MyResults;

namespace ID.Application.Features.Account.Cmd.Cookies.TwoFactorResendCookie;
public class Resend2FactorCookieHandler(
    ITwoFactorMsgService twoFactorMsgHandler,
    ITwofactorUserIdCacheService _twofactorUserIdCache,
    IFindUserService<AppUser> _findUserService,
    ICookieAuthService<AppUser> _cookieSignInService,
    ITwoFactorVerificationService<AppUser> _2FactorService)
    : IIdCommandHandler<Resend2FactorCookieCmd>
{

    public async Task<BasicResult> Handle(Resend2FactorCookieCmd request, CancellationToken cancellationToken)
    {

        //return BasicResult.Success();
        var token = _cookieSignInService.TryGetTwoFactorToken();
        var rememberMe = _cookieSignInService.GetRememberMe();
        if (string.IsNullOrWhiteSpace(token))
            return BasicResult.BadRequestResult(IDMsgs.Error.TwoFactor.INVALID_2_FACTOR_TOKEN);

        var userId = _twofactorUserIdCache.GetUserId(token);
        var user = await _findUserService.FindUserWithTeamDetailsAsync(userId: userId);
        var team = user?.Team;

        if (user is null || team is null)
            return GenResult<MfaResultData>.BadRequestResult(IDMsgs.Error.TwoFactor.INVALID_2_FACTOR_TOKEN); //Just blame it on the token no more info should be revealed

        if (!await _2FactorService.IsTwoFactorEnabledAsync(user))
            return GenResult<MfaResultData>.BadRequestResult(IDMsgs.Error.TwoFactor.MULTI_FACTOR_NOT_ENABLED);


        //await _cookieSignInService.CreateWithTwoFactorRequiredAsync(
        //   rememberMe,
        //   user);


        await twoFactorMsgHandler.SendOTPFor2FactorAuth(team, user);


        return BasicResult.Success(IDMsgs.Info.TwoFactor.TWO_FACTOR_HAS_BEEN_SENT(user));

        //var dto = request.Dto;
        //var userId = _twofactorUserIdCache.GetUserId(dto.Token);
        //var user = await _findUserService.FindUserWithTeamDetailsAsync(userId: userId);
        //var team = user?.Team;


        //if (user is null || team is null)
        //    return GenResult<MfaResultData>.BadRequestResult(IDMsgs.Error.TwoFactor.INVALID_2_FACTOR_TOKEN); //Just blame it on the token no more info should be revealed

        //if (!await _2FactorService.IsTwoFactorEnabledAsync(user))
        //    return GenResult<MfaResultData>.BadRequestResult(IDMsgs.Error.TwoFactor.NO_MSG_PROVIDER_SET);

        //return await twoFactorMsgHandler.SendOTPFor2FactorAuth(team, user);
    }


}//Cls
