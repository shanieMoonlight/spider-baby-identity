using ID.Application.Features.Common.Dtos.User;
using ID.Application.Dtos.User;
using MyResults;
using ID.Application.AppAbs.TokenVerificationServices;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Features.Account.Cmd.Mfa.TwoFactorEnable;
public class TwoFactorEnableHandler(ITwoFactorVerificationService<AppUser> _2FactorService) : IIdCommandHandler<TwoFactorEnableCmd, AppUserDto>
{

    public async Task<GenResult<AppUserDto>> Handle(TwoFactorEnableCmd request, CancellationToken cancellationToken)
    {
        var user = request.PrincipalUser!;//AUserAwareCommand ensures that PrincipalUser is not null

        var enableResult = await _2FactorService.EnableTwoFactorTokenAsync(user);
        return enableResult.Convert(u => u?.ToDto());

    }

}//Cls
