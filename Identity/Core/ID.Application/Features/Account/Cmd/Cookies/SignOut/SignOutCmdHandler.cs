using ID.Application.Mediatr.CqrsAbs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using MyResults;

namespace ID.Application.Features.Account.Cmd.Cookies.SignOut;
public class SignOutCmdHandler(IHttpContextAccessor httpContextAccessor) : IIdCommandHandler<SignOutCmd>
{

    public async Task<BasicResult> Handle(SignOutCmd request, CancellationToken cancellationToken)
    {
        await httpContextAccessor.HttpContext!.SignOutAsync();
        return  BasicResult.Success("Sign out successful!");
    }

}//Cls
