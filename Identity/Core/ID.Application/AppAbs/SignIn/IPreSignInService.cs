using ID.Application.Features.Account.Cmd.Login;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.AppAbs.SignIn;
public interface IPreSignInService<TUser> where TUser : AppUser
{
    Task<MyIdSignInResult> Authenticate(LoginDto dto, CancellationToken cancellationToken);
}