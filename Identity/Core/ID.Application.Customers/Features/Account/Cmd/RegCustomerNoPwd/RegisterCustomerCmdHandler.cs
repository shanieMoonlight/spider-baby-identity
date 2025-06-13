using ID.Application.Customers.Abstractions;
using ID.Application.Customers.Features.Common.Dtos.User;
using ID.Application.Mediatr.CqrsAbs;
using MyResults;

namespace ID.Application.Customers.Features.Account.Cmd.RegCustomerNoPwd;
public class RegisterCustomerNoPwdCmdHandler(IIdCustomerRegistrationService regService)
    : IIdCommandHandler<RegisterCustomerNoPwdCmd, AppUser_Customer_Dto>
{
    public async Task<GenResult<AppUser_Customer_Dto>> Handle(RegisterCustomerNoPwdCmd request, CancellationToken cancellationToken)
    {
        var createResult = await regService.Register_NoPwd_Async(request.Dto, cancellationToken);

        return createResult.Convert(u => u?.ToCustomerDto());
    }


}//Cls



