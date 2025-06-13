using ID.Application.Customers.Features.Common.Dtos.User;
using ID.Application.Mediatr.Cqrslmps.Commands;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Customers.Features.Account.Cmd.AddCustomerMember;
public record AddCustomerMemberCmd(AddCustomerMemberDto Dto)
    : AIdUserAndTeamAwareCommand<AppUser, AppUser_Customer_Dto>;



