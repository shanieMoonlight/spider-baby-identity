using ID.Application.Features.Common.Dtos.User;
using ID.Application.Dtos.User;
using MyResults;
using ID.Application.AppAbs.MFA.AuthenticatorApps;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Utility.Messages;

namespace ID.Application.Features.Account.Cmd.Mfa.Reg.TwoFactorGoogleComplete;
public class MfaGoogleCompleteRegistrationCmdHandler(IAuthenticatorAppService googleAuth)
    : IIdCommandHandler<MfaGoogleCompleteRegistrationCmd, AppUserDto>
{

    public async Task<GenResult<AppUserDto>> Handle(MfaGoogleCompleteRegistrationCmd request, CancellationToken cancellationToken)
    {
        var user = request.PrincipalUser;

        var enableResult = await googleAuth.EnableAsync(user, request.TwoFactorCode);

        return !enableResult.Succeeded
            ? GenResult<AppUserDto>.Failure(IDMsgs.Error.TwoFactor.INVALID_2_FACTOR_TOKEN)
            : GenResult<AppUserDto>.Success(user.ToDto());
    }

}//Cls
