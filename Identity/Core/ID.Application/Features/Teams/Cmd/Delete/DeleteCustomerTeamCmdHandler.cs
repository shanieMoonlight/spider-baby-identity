using MyResults;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Utility.Messages;
using ID.Domain.Entities.Teams;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Abstractions.Services.Teams;

namespace ID.Application.Features.Teams.Cmd.Delete;
public class DeleteCustomerTeamCmdHandler(IIdentityTeamManager<AppUser> teamMgr) : IIdCommandHandler<DeleteCustomerTeamCmd>
{

    public async Task<BasicResult> Handle(DeleteCustomerTeamCmd request, CancellationToken cancellationToken)
    {
        var id = request.Id;
        var team = await teamMgr.GetByIdWithMembersAsync(id, 1000);

        if (team == null)
            return BasicResult.NotFoundResult(IDMsgs.Error.NotFound<Team>(id));


        if (!team.IsCustomer())
            return BasicResult.BadRequestResult(IDMsgs.Error.Teams.CAN_ONLY_REMOVE_CUSTOMER_TEAM);


        return await teamMgr.DeleteTeamAsync(team);
    }

}//Cls


