using ID.Application.Features.Common.Dtos.User;
using ID.Application.Dtos.User;
using MyResults;
using ID.Application.AppAbs.ApplicationServices.TwoFactor;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Utility.Messages;

namespace ID.Application.Features.Account.Cmd.Mfa.Reg.TwoFactorAuthAppEmailComplete;
public class TwoFactorAuthAppEmailCompleteCmdHandler(ITwoFactorCompleteRegistrationHandler regCompleteHandler)
    : IIdCommandHandler<TwoFactorAuthAppEmailCompleteCmd, AppUserDto>
{

    public async Task<GenResult<AppUserDto>> Handle(TwoFactorAuthAppEmailCompleteCmd request, CancellationToken cancellationToken)
    {
        var twoFactorCode = request.TwoFactorCode;

        var user = request.PrincipalUser!; //AUserAwareCommand ensures that this is not null


        var enableResult = await regCompleteHandler.EnableAsync(user, twoFactorCode);

        if (!enableResult.Succeeded)
            return GenResult<AppUserDto>.Failure(IDMsgs.Error.TwoFactor.INVALID_2_FACTOR_CODE);

        return GenResult<AppUserDto>.Success(user.ToDto());
    }

}//Cls
