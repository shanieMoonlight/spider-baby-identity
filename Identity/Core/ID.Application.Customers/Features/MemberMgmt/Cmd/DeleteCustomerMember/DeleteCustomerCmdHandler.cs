using ID.Application.AppAbs.Permissions;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Entities.AppUsers;
using MyResults;

namespace ID.Application.Customers.Features.MemberMgmt.Cmd.DeleteCustomerMember;
public class DeleteCustomerMemberCmdHandler(
    IIdentityTeamManager<AppUser> _teamMgr,
    IAppPermissionService<AppUser> _appPermissions) : IIdCommandHandler<DeleteCustomerMemberCmd>
{

    public async Task<BasicResult> Handle(DeleteCustomerMemberCmd request, CancellationToken cancellationToken)
    {

        var deleteUserId = request.UserId;
        var canDeleteUserResult = await _appPermissions.DeletePermissions
            .CanDeleteAsync(deleteUserId, request);

        if (!canDeleteUserResult.Succeeded)
            return canDeleteUserResult.ToBasicResult();

        var deleteUser = canDeleteUserResult.Value!; //Success is non-null

        var team = request.PrincipalTeam;//AIdUserAndTeamAwareCommand ensures that this is not null


        return await _teamMgr.DeleteMemberAsync(team, deleteUser.Id);
    }

}//Cls


