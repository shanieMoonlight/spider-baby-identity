using ID.Application.AppAbs.ApplicationServices.TwoFactor;
using ID.Application.AppAbs.ApplicationServices.User;
using ID.Application.AppAbs.TokenVerificationServices;
using ID.Application.Mediatr.CqrsAbs;
using ID.Application.MFA;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Utility.Messages;
using MyResults;

namespace ID.Application.Features.Account.Cmd.Mfa.TwoFactorResend;
public class Resend2FactorHandler(
    ITwoFactorMsgService twoFactorMsgHandler,
    ITwofactorUserIdCacheService _twofactorUserIdCache,
    IFindUserService<AppUser> _findUserService,
    ITwoFactorVerificationService<AppUser> _2FactorService)
    : IIdCommandHandler<Resend2FactorCmd, MfaResultData>
{

    public async Task<GenResult<MfaResultData>> Handle(Resend2FactorCmd request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var userId = _twofactorUserIdCache.GetUserId(dto.Token);
        var user = await _findUserService.FindUserWithTeamDetailsAsync(userId: userId);
        var team = user?.Team;


        if (user is null || team is null)
            return GenResult<MfaResultData>.BadRequestResult(IDMsgs.Error.TwoFactor.INVALID_2_FACTOR_TOKEN); //Just blame it on the token no more info should be revealed

        if (!await _2FactorService.IsTwoFactorEnabledAsync(user))
            return GenResult<MfaResultData>.BadRequestResult(IDMsgs.Error.TwoFactor.MULTI_FACTOR_NOT_ENABLED);

        return await twoFactorMsgHandler.SendOTPFor2FactorAuth(team, user);
    }


}//Cls
