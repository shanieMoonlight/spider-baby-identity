using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.Teams;
using ID.Domain.Utility.Messages;
using ID.GlobalSettings.Constants;
using MyResults;

namespace ID.Application.Features.Teams.Qry.GetSuperTeam;
internal class GetSuperTeamQryHandler() : IIdQueryHandler<GetSuperTeamQry, TeamDto>
{
    public Task<GenResult<TeamDto>> Handle(GetSuperTeamQry request, CancellationToken cancellationToken)
    {
        var mdl = request.PrincipalTeam; //Only super team members can get here
        if (mdl == null)
            return Task.FromResult(GenResult<TeamDto>.NotFoundResult(IDMsgs.Error.NotFound<Team>(IdGlobalConstants.Teams.SUPER_TEAM_NAME)));

        return Task.FromResult(GenResult<TeamDto>.Success(mdl.ToDto()));

    }

}//Cls
