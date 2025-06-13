using ID.Application.Customers.Features.Common.Dtos.User;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Abstractions.Services.Members;
using ID.Domain.Entities.AppUsers;
using MyResults;
using Pagination;

namespace ID.Application.Customers.Features.MemberMgmt.Qry.GetCustomersPage;

internal class GetCustomersPageQryHandler(IIdentityMemberAuditService<AppUser> customerAuditService)
    : IIdQueryHandler<GetCustomersPageQry, PagedResponse<AppUser_Customer_Dto>>
{

    public async Task<GenResult<PagedResponse<AppUser_Customer_Dto>>> Handle(GetCustomersPageQry request, CancellationToken cancellationToken)
    {
        var pgRequest = request.PagedRequest ?? PagedRequest.Empty();
        var page = await customerAuditService.GetCustomerPageAsync(request.PagedRequest ?? PagedRequest.Empty());

        var pgResponse = new PagedResponse<AppUser>(page, pgRequest)
            .Transform(d => d.ToCustomerDto());

        return GenResult<PagedResponse<AppUser_Customer_Dto>>.Success(pgResponse);
    }

}//Cls

