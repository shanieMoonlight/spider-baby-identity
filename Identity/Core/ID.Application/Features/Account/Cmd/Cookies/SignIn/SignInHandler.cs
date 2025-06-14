using ID.Application.AppAbs.SignIn;
using ID.Domain.Entities.AppUsers;
using MediatR;

namespace ID.Application.Features.Account.Cmd.Cookies.SignIn;
public class SignInHandler(ICookieSignInService<AppUser> _cookieSignInService)
    : IRequestHandler<SignInCmd, MyIdSignInResult>
{

    public async Task<MyIdSignInResult> Handle(SignInCmd request, CancellationToken cancellationToken) =>
        await _cookieSignInService.PasswordSignInAsync(request.Dto, cancellationToken);

}//Cls

