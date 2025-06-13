using ID.Application.Customers.Features.Common.Dtos.User;
using ID.Application.Customers.Dtos.User;
using ID.Application.Mediatr.Cqrslmps.Queries;

namespace ID.Application.Customers.Features.MemberMgmt.Qry.GetCustomer;
public record GetCustomerQry(Guid TeamId, Guid MemberId) : AIdQuery<AppUser_Customer_Dto>;



