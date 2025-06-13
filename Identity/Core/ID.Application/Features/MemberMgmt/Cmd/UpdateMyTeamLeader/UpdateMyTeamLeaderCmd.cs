using ID.Application.Features.Teams;
using ID.Application.Mediatr.Cqrslmps.Commands;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Features.MemberMgmt.Cmd.UpdateMyTeamLeader;
public record UpdateMyTeamLeaderCmd(Guid NewLeaderId) : AIdUserAndTeamAwareCommand<AppUser, TeamDto>;



