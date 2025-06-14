using ID.Application.Utility.Attributes;
using ID.Application.Features.Account.Cmd.Login;

namespace ID.Application.Features.Account.Cmd.Cookies.SignIn;

[AtLeastOneProperty(nameof(Username), nameof(UserId), nameof(Email))]
public class CookieSignInDto : LoginDto
{
    public bool RememberMe { get; set; }

 
}//Cls