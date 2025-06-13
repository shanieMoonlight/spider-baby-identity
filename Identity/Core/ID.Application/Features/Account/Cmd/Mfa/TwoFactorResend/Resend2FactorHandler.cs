using MyResults;
using ID.Application.AppAbs.TokenVerificationServices;
using ID.Application.AppAbs.ApplicationServices.TwoFactor;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Utility.Messages;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Features.Account.Cmd.Mfa.TwoFactorResend;
public class Resend2FactorHandler(
    ITwoFactorMsgService twoFactorMsgHandler,
    ITwoFactorVerificationService<AppUser> _2FactorService)
    : IIdCommandHandler<Resend2FactorCmd, MfaResultData>
{

    public async Task<GenResult<MfaResultData>> Handle(Resend2FactorCmd request, CancellationToken cancellationToken)
    {
        //User is already logged in to get to the TwoFactor code Page
        var user = request.PrincipalUser;
        var team = request.PrincipalTeam;

        if (!await _2FactorService.IsTwoFactorEnabledAsync(user))
            return GenResult<MfaResultData>.BadRequestResult(IDMsgs.Error.TwoFactor.NO_MSG_PROVIDER_SET);

        return await twoFactorMsgHandler.SendOTPFor2FactorAuth(team, user);
    }


}//Cls
