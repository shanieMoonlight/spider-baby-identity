using ID.Application.Features.Common.Dtos.User;
using ID.Application.Dtos.User;
using MyResults;
using ID.Application.AppAbs.ApplicationServices.TwoFactor;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Utility.Messages;

namespace ID.Application.Features.Account.Cmd.Mfa.Reg.TwoFactorAuthAppComplete;
public class TwoFactorAuthAppCompleteRegCmdHandler(ITwoFactorCompleteRegistrationHandler regCompleteHandler)
    : IIdCommandHandler<TwoFactorAuthAppCompleteRegCmd, AppUserDto>
{

    public async Task<GenResult<AppUserDto>> Handle(TwoFactorAuthAppCompleteRegCmd request, CancellationToken cancellationToken)
    {
        var twoFactorCode = request.Dto.TwoFactorCode;
        var customerSecretKey = request.Dto.CustomerSecretKey;

        var user = request.PrincipalUser!; //AUserAwareCommand ensures that this is not null

        user.SetTwoFactorKey(customerSecretKey);


        var enableResult = await regCompleteHandler.EnableAsync(user, twoFactorCode);

        return !enableResult.Succeeded
            ? GenResult<AppUserDto>.Failure(IDMsgs.Error.TwoFactor.INVALID_2_FACTOR_CODE)
            : GenResult<AppUserDto>.Success(user.ToDto());

    }

}//Cls
