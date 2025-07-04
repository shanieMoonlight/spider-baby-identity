using ID.Application.Features.Common.Dtos.User;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Domain.Utility.Messages;
using MyResults;

namespace ID.Application.Customers.Features.Account.Cmd.CloseAccount;
public class CloseAccountCmdHandler(IIdentityTeamManager<AppUser> teamMgr)
    : IIdCommandHandler<CloseAccountCmd>
{

    public async Task<BasicResult> Handle(CloseAccountCmd request, CancellationToken cancellationToken)
    {
        var principalTeam = request.PrincipalTeam!;
        var teamId = request.TeamId;

        if (principalTeam?.TeamType == TeamType.customer || request.IsCustomer)
            return GenResult<AppUserDto>.UnauthorizedResult();


        var team = await teamMgr.GetByIdWithEverythingAsync(teamId);


        if (team is null)
            return GenResult<AppUserDto>.NotFoundResult(IDMsgs.Error.NotFound<Team>(teamId));

        return await teamMgr.DeleteTeamAsync(team);

    }

}//Cls

