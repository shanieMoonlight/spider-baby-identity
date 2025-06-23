using ID.Application.AppAbs.MFA.AuthenticatorApps;
using ID.Application.Dtos.User;
using ID.Application.Features.Common.Dtos.User;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Utility.Messages;
using MyResults;

namespace ID.Application.Features.Account.Cmd.Mfa.Reg.TwoFactorAuthyComplete;
public class TwoFactorAuthyCompleteCmdHandler(IAuthenticatorAppService authy)
    : IIdCommandHandler<TwoFactorAuthyCompleteRegCmd, AppUserDto>
{

    public async Task<GenResult<AppUserDto>> Handle(TwoFactorAuthyCompleteRegCmd request, CancellationToken cancellationToken)
    {
        var twoFactorCode = request.TwoFactorCode;

        var user = request.PrincipalUser!; //AUserAwareCommand ensures that this is not null


        var enableResult = await authy.EnableAsync(user, twoFactorCode);

        return !enableResult.Succeeded
            ? GenResult<AppUserDto>.Failure(IDMsgs.Error.TwoFactor.INVALID_2_FACTOR_CODE)
            : GenResult<AppUserDto>.Success(user.ToDto());
    }

}//Cls
