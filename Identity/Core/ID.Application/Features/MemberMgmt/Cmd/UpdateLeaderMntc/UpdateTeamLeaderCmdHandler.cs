using MyResults;
using ID.Application.AppAbs.Permissions;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Abstractions.Services.Teams;
using ID.Application.Features.Teams;

namespace ID.Application.Features.MemberMgmt.Cmd.UpdateLeaderMntc;
public class UpdateTeamLeaderCmdHandler(IIdentityTeamManager<AppUser> teamMgr, IAppPermissionService<AppUser> _appPermissions)
    : IIdCommandHandler<UpdateTeamLeaderCmd, TeamDto>
{

    public async Task<GenResult<TeamDto>> Handle(UpdateTeamLeaderCmd request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var newLeaderId = dto.NewLeaderId;
        var teamId = dto.TeamId;

        var canChangeLeaderResult = await _appPermissions.ChangeLeaderPermissions
            .CanChange_SpecifiedTeam_LeaderAsync(teamId, newLeaderId, request);

        if (!canChangeLeaderResult.Succeeded)
            return canChangeLeaderResult.Convert<TeamDto>();

        var newLeaderTeam = canChangeLeaderResult.Value!; //Success is non-null
        var newLeader = newLeaderTeam.Members.First();

        var updateResult = await teamMgr.SetLeaderAsync(newLeaderTeam, newLeader);
        return updateResult.Convert(t => t?.ToDto());
    }

}//Cls




