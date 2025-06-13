using ID.Application.Features.Teams;
using ID.Application.Mediatr.Cqrslmps.Commands;

namespace ID.Application.Features.Teams.Cmd.Create;
public record CreateCustomerTeamCmd(TeamDto Dto) : AIdCommand<TeamDto>;



