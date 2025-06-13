using MyResults;
using ID.Application.AppAbs.Permissions;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Utility.Messages;
using ID.GlobalSettings.Constants;
using ID.Domain.Entities.Teams;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Abstractions.Services.Teams;

namespace ID.Application.Features.MemberMgmt.Cmd.DeleteSuperMember;
public class DeleteSprMemberCmdHandler(IIdentityTeamManager<AppUser> _teamMgr, IAppPermissionService<AppUser> _appPermissions)
    : IIdCommandHandler<DeleteSprMemberCmd>
{

    public async Task<BasicResult> Handle(DeleteSprMemberCmd request, CancellationToken cancellationToken)
    {
        var deleteUserId = request.UserId;
        var canDeleteUserResult = await _appPermissions.DeletePermissions
            .CanDeleteAsync(deleteUserId, request);

        if (!canDeleteUserResult.Succeeded)
            return canDeleteUserResult.ToBasicResult();

        var deleteUser = canDeleteUserResult.Value!; //Success is non-null

        var team = await _teamMgr.GetSuperTeamWithMembersAsync();
        if (team is null)
            return BasicResult.NotFoundResult(IDMsgs.Error.NotFound<Team>(IdGlobalConstants.Teams.SUPER_TEAM_NAME));


        return await _teamMgr.DeleteMemberAsync(team, deleteUser.Id);
    }


}//Cls

