using ID.Application.Features.Teams;
using ID.Application.Mediatr.Cqrslmps.Queries;

namespace ID.Application.Features.Teams.Qry.GetById;
public record GetTeamByIdQry(Guid? Id) : AIdQuery<TeamDto>;
