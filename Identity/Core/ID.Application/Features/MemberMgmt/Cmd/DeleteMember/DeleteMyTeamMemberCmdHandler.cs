using ID.Application.AppAbs.Permissions;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Entities.AppUsers;
using MyResults;

namespace ID.Application.Features.MemberMgmt.Cmd.DeleteMember;
public class DeleteMyTeamMemberCmdHandler(IIdentityTeamManager<AppUser> _teamMgr, IAppPermissionService<AppUser> _appPermissions) : IIdCommandHandler<DeleteMyTeamMemberCmd>
{

    public async Task<BasicResult> Handle(DeleteMyTeamMemberCmd request, CancellationToken cancellationToken)
    {
        var deleteUserId = request.UserId;
        var canDeleteUserResult = await _appPermissions.DeletePermissions
            .CanDeleteAsync(deleteUserId, request);

        if (!canDeleteUserResult.Succeeded)
            return canDeleteUserResult.ToBasicResult();

        var deleteUser = canDeleteUserResult.Value!; //Success is non-null

        var team = request.PrincipalTeam;


        return await _teamMgr.DeleteMemberAsync(team, deleteUser.Id);
    }

}//Cls

