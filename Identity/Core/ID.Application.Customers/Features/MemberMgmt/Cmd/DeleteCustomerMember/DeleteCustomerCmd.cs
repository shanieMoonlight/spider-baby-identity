using ID.Application.Mediatr.Cqrslmps.Commands;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Customers.Features.MemberMgmt.Cmd.DeleteCustomerMember;
public record DeleteCustomerMemberCmd(Guid UserId) : AIdUserAndTeamAwareCommand<AppUser>;



