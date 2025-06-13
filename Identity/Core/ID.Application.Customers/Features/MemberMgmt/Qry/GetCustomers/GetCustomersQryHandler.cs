using ID.Application.Customers.Features.Common.Dtos.User;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Abstractions.Services.Members;
using ID.Domain.Entities.AppUsers;
using MyResults;

namespace ID.Application.Customers.Features.MemberMgmt.Qry.GetCustomers;

internal class GetCustomersQryHandler(IIdentityMemberAuditService<AppUser> customerAuditService)
    : IIdQueryHandler<GetCustomersQry, IEnumerable<AppUser_Customer_Dto>>
{

    public async Task<GenResult<IEnumerable<AppUser_Customer_Dto>>> Handle(GetCustomersQry request, CancellationToken cancellationToken)
    {
        var customers = await customerAuditService.GetCustomersAsync();
        var dtos = customers.Select(c => c.ToCustomerDto());

        return GenResult<IEnumerable<AppUser_Customer_Dto>>.Success(dtos);
    }

}//Cls

