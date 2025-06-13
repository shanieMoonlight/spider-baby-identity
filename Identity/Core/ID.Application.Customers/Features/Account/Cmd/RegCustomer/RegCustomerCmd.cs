using ID.Application.Customers.Features.Common.Dtos.User;
using ID.Application.Customers.Dtos.User;
using ID.Application.Mediatr.Cqrslmps.Commands;

namespace ID.Application.Customers.Features.Account.Cmd.RegCustomer;

/// <summary>
/// Register Customer. (No OAuth)
/// </summary>
public record RegisterCustomerCmd(RegisterCustomerDto Dto)
    : AIdCommand<AppUser_Customer_Dto>;



