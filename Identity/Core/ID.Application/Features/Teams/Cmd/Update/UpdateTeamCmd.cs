using ID.Application.Features.Teams;
using ID.Application.Mediatr.Cqrslmps.Commands;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Features.Teams.Cmd.Update;
public record UpdateTeamCmd(TeamDto Dto) : AIdUserAndTeamAwareCommand<AppUser, TeamDto>;



