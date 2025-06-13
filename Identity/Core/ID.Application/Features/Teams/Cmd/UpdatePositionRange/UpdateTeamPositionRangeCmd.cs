using ID.Application.Features.Teams;
using ID.Application.Mediatr.Cqrslmps.Commands;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Features.Teams.Cmd.UpdatePositionRange;


public record UpdateTeamPositionRangeDto(int MinPosition, int MaxPosition);

public record UpdateTeamPositionRangeCmd(UpdateTeamPositionRangeDto Dto) : AIdUserAndTeamAwareCommand<AppUser, TeamDto>;



