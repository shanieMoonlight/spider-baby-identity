using ID.Application.Features.Common.Dtos.User;
using ID.Application.Dtos.User;
using MyResults;
using ID.Application.AppAbs.TokenVerificationServices;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Features.Account.Cmd.Mfa.TwoFactorDisable;
public class TwoFactorDisableHandler(ITwoFactorVerificationService<AppUser> _2FactorService)
    : IIdCommandHandler<TwoFactorDisableCmd, AppUserDto>
{
    public async Task<GenResult<AppUserDto>> Handle(TwoFactorDisableCmd request, CancellationToken cancellationToken)
    {
        var user = request.PrincipalUser!;//AUserAwareCommand ensures that PrincipalUser is not null

        var disableResult = await _2FactorService.DisableTwoFactorTokenAsync(user);
        return disableResult.Convert(u => u?.ToDto());
    }

}//Cls
