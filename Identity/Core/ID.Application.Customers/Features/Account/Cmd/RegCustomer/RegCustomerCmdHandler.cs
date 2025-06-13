using ID.Application.Customers.Abstractions;
using ID.Application.Customers.Features.Common.Dtos.User;
using ID.Application.Mediatr.CqrsAbs;
using MyResults;

namespace ID.Application.Customers.Features.Account.Cmd.RegCustomer;
public class RegCustomerCmdHandler(IIdCustomerRegistrationService regService)
    : IIdCommandHandler<RegisterCustomerCmd, AppUser_Customer_Dto>
{

    public async Task<GenResult<AppUser_Customer_Dto>> Handle(RegisterCustomerCmd request, CancellationToken cancellationToken)
    {
        var createResult = await regService.RegisterAsync(request.Dto, cancellationToken);

        return createResult.Convert(u => u?.ToCustomerDto());
    }

}//Cls

