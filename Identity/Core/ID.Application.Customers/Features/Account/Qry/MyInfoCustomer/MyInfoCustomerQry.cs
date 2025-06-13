using ID.Application.Customers.Features.Common.Dtos.User;
using ID.Application.Mediatr.Cqrslmps.Queries;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Customers.Features.Account.Qry.MyInfoCustomer;
public record MyInfoCustomerQry(object? Dto = null) : AIdUserAwareQuery<AppUser, AppUser_Customer_Dto>;



