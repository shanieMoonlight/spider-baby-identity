using ID.Application.Dtos.User;
using MyResults;
using ID.Application.Features.Teams;
using ID.Application.AppAbs.Permissions;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Abstractions.Services.Teams;

namespace ID.Application.Features.MemberMgmt.Cmd.UpdateMyTeamLeader;


public class UpdateMyTeamLeaderCmdHandler(IIdentityTeamManager<AppUser> teamMgr, IAppPermissionService<AppUser> _appPermissions)
    : IIdCommandHandler<UpdateMyTeamLeaderCmd, TeamDto>
{

    public async Task<GenResult<TeamDto>> Handle(UpdateMyTeamLeaderCmd request, CancellationToken cancellationToken)
    {
        var newLeaderId = request.NewLeaderId;

        var team = request.PrincipalTeam;

        var canChangeLeaderResult = await _appPermissions.ChangeLeaderPermissions
            .CanChange_MyTeam_LeaderAsync(newLeaderId, request);
        if (!canChangeLeaderResult.Succeeded)
            return canChangeLeaderResult.Convert<TeamDto>();


        var newLeader = canChangeLeaderResult.Value!; //Success is non-null

        var updateResult = await teamMgr.SetLeaderAsync(team, newLeader);
        return updateResult.Convert(t => t?.ToDto());
    }

}//Cls

