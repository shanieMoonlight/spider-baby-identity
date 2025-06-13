using ID.Application.Mediatr.Cqrslmps.Commands;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Features.MemberMgmt.Cmd.DeleteMntcMember;

/// <summary>
/// Delete a member of your own the team. 
/// The Deleter must be the leader or have a higher position that the Deletee.
/// </summary>
/// <param name="UserId">Team Member to Delete's identifier</param>
public record DeleteMntcMemberCmd(Guid UserId) : AIdUserAndTeamAwareCommand<AppUser>;



