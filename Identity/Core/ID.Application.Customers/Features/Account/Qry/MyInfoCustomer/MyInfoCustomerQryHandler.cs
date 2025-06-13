using ID.Application.Customers.Features.Common.Dtos.User;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.AppUsers;
using MyResults;

namespace ID.Application.Customers.Features.Account.Qry.MyInfoCustomer;
public class MyInfoCustomerQryHandler()
    : IIdQueryHandler<MyInfoCustomerQry, AppUser_Customer_Dto>
{

    public Task<GenResult<AppUser_Customer_Dto>> Handle(MyInfoCustomerQry request, CancellationToken cancellationToken)
    {

        AppUser user = request.PrincipalUser!; // UserAwarePipelineBehavior ensures this is not null

        return Task.FromResult(GenResult<AppUser_Customer_Dto>.Success(user.ToCustomerDto()));

    }

}//Cls
