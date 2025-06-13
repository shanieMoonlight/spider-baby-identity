using ID.Application.Features.Teams;
using ID.Application.Mediatr.Cqrslmps.Queries;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Features.Teams.Qry.GetSuperTeam;
public record GetSuperTeamQry() : AIdUserAndTeamAwareQuery<AppUser, TeamDto>;
