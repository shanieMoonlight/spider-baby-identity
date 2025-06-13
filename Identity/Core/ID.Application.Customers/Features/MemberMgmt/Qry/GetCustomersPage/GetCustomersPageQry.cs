using ID.Application.Customers.Features.Common.Dtos.User;
using ID.Application.Mediatr.Cqrslmps.Queries;
using Pagination;

namespace ID.Application.Customers.Features.MemberMgmt.Qry.GetCustomersPage;
public record GetCustomersPageQry(PagedRequest? PagedRequest)
    : AIdQuery<PagedResponse<AppUser_Customer_Dto>>;



