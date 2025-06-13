using ID.Application.Mediatr.Cqrslmps.Queries;

namespace ID.Application.Features.Teams.Qry.GetByName;

public record GetTeamsByNameQry(string Name) : AIdQuery<IEnumerable<TeamDto>>;



