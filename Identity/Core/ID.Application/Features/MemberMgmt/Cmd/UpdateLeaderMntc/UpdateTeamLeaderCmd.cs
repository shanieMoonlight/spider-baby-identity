using ID.Application.Features.Teams;
using ID.Application.Mediatr.Cqrslmps.Commands;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Features.MemberMgmt.Cmd.UpdateLeaderMntc;

/// <summary>
/// Represents a command to update the team leader for a specified team.
/// For use by Super Or Maintenance teams to update the leader of their own team or a lower team.
/// </summary>
/// <remarks>This command encapsulates the data required to update the team leader, including user and team
/// context. It is typically used in scenarios where the team leader needs to be reassigned or updated.</remarks>
/// <param name="Dto">The data transfer object containing the necessary information to update the team leader. </param>
public record UpdateTeamLeaderCmd(UpdateTeamLeaderDto Dto) : AIdUserAndTeamAwareCommand<AppUser, TeamDto>;



