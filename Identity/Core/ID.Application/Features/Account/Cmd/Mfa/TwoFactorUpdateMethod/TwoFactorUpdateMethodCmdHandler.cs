using ID.Application.Features.Common.Dtos.User;
using ID.Application.Dtos.User;
using MyResults;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Abstractions.Services.Teams;

namespace ID.Application.Features.Account.Cmd.Mfa.TwoFactorUpdateMethod;
public class TwoFactorUpdateMethodCmdHandler(IIdentityTeamManager<AppUser> teamMgr)
    : IIdCommandHandler<TwoFactorUpdateMethodCmd, AppUserDto>
{

    public async Task<GenResult<AppUserDto>> Handle(TwoFactorUpdateMethodCmd request, CancellationToken cancellationToken)
    {
        var provider = request.Provider!.Value;

        var user = request.PrincipalUser!; //AUserAwareCommand ensures that this is not null
        var team = request.PrincipalTeam!; //AUserAndTeamAwareCommand ensures that this is not null

        user.Update2FactorProvider(provider);

        var updateResult = await teamMgr.UpdateMemberAsync(team, user);
        if (!updateResult.Succeeded)
            return updateResult.Convert<AppUserDto>();

        return GenResult<AppUserDto>.Success(updateResult.Value!.ToDto());

    }

}//Cls
