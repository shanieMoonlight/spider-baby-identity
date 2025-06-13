using ID.Application.Features.Common.Dtos.User;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Entities.AppUsers;
using MyResults;

namespace ID.Application.Customers.Features.Account.Cmd.CloseAccount;
public class CloseMyAccountCmdHandler(IIdentityTeamManager<AppUser> teamMgr)
    : IIdCommandHandler<CloseMyAccountCmd>
{

    public async Task<BasicResult> Handle(CloseMyAccountCmd request, CancellationToken cancellationToken)
    {
        var team = request.PrincipalTeam!;

        //Only leader or Higher team member can delete a team
        if (!request.IsLeader)
            return GenResult<AppUserDto>.UnauthorizedResult();

        return await teamMgr.DeleteTeamAsync(team);

    }

}//Cls

