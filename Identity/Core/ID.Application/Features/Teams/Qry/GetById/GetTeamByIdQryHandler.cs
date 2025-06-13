using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Domain.Utility.Messages;
using MyResults;

namespace ID.Application.Features.Teams.Qry.GetById;
internal class GetTeamByIdQryHandler(IIdentityTeamManager<AppUser> _mgr) : IIdQueryHandler<GetTeamByIdQry, TeamDto>
{
    public async Task<GenResult<TeamDto>> Handle(GetTeamByIdQry request, CancellationToken cancellationToken)
    {
        var teamId = request.Id;
        var position = request.PrincipalTeamId == teamId
            ? request.PrincipalTeamPosition
            : 1000;


        var dbTeam = await _mgr.GetByIdWithEverythingAsync(teamId, position);
        if (dbTeam == null)
            return GenResult<TeamDto>.NotFoundResult(IDMsgs.Error.NotFound<Team>(teamId));

        //Only Mntc or higher can get here. Make sure Mntc can't access Super teams
        if (dbTeam.IsSuper() && !request.IsSuper)
            return GenResult<TeamDto>.ForbiddenResult(IDMsgs.Error.Teams.UNAUTHORIZED_FOR_SUPER_TEAM(1));


        return GenResult<TeamDto>.Success(dbTeam.ToDto());

    }

}//Cls
