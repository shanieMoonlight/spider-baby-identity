using ID.Application.Features.Teams;
using ID.Application.Mediatr.Cqrslmps.Queries;

namespace ID.Application.Features.Teams.Qry.GetAll;
public record GetAllTeamsQry : AIdQuery<IEnumerable<TeamDto>>;
