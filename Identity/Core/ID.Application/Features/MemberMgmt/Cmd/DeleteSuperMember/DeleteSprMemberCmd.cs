using ID.Application.Mediatr.Cqrslmps.Commands;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Features.MemberMgmt.Cmd.DeleteSuperMember;
public record DeleteSprMemberCmd(Guid UserId) : AIdUserAndTeamAwareCommand<AppUser>;



