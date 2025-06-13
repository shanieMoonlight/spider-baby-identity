using ID.Application.Customers.Features.Common.Dtos.User;
using ID.Application.Customers.Dtos.User;
using ID.Application.Mediatr.Cqrslmps.Queries;

namespace ID.Application.Customers.Features.MemberMgmt.Qry.GetCustomers;
public record GetCustomersQry : AIdQuery<IEnumerable<AppUser_Customer_Dto>>;



