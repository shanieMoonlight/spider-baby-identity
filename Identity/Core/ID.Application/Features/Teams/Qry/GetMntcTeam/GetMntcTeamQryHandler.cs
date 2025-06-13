using MyResults;
using ID.Application.Features.Teams;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Utility.Messages;
using ID.GlobalSettings.Constants;
using ID.Domain.Entities.Teams;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Abstractions.Services.Teams;

namespace ID.Application.Features.Teams.Qry.GetMntcTeam;
internal class GetMntcTeamQryHandler(IIdentityTeamManager<AppUser> _mgr) : IIdQueryHandler<GetMntcTeamQry, TeamDto>
{
    public async Task<GenResult<TeamDto>> Handle(GetMntcTeamQry request, CancellationToken cancellationToken)
    {
        var teamPosition = request.IsSuperMinimum
            ? IdGlobalConstants.Teams.CATCH_ALL_MAX_POSITION
            : request.PrincipalTeamPosition;

        var mdl = await _mgr.GetMntcTeamWithMembersAsync(teamPosition);
        if (mdl == null)
            return GenResult<TeamDto>.NotFoundResult(IDMsgs.Error.NotFound<Team>(IdGlobalConstants.Teams.MAINTENANCE_TEAM_NAME));

        return GenResult<TeamDto>.Success(mdl.ToDto());

    }

}//Cls
