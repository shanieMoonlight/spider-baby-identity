using ID.Application.Mediatr.Cqrslmps.Commands;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Features.MemberMgmt.Cmd.DeleteMember;

/// <summary>
/// Only Mntc Members can delete a Mntc Member
/// </summary>
/// <param name="UserId"></param>
public record DeleteMyTeamMemberCmd(Guid UserId) : AIdUserAndTeamAwareCommand<AppUser>;



