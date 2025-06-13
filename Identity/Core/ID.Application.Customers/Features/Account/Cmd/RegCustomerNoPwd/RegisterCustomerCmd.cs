using ID.Application.Customers.Dtos.User;
using ID.Application.Customers.Features.Common.Dtos.User;
using ID.Application.Mediatr.Cqrslmps.Commands;

namespace ID.Application.Customers.Features.Account.Cmd.RegCustomerNoPwd;
public record RegisterCustomerNoPwdCmd(RegisterCustomer_NoPwdDto Dto) : AIdCommand<AppUser_Customer_Dto>;



