using ID.OAuth.Facebook.Features.SignIn;

namespace ID.OAuth.Facebook.Features.SignIn.FacebookCookieSignIn;

public sealed class FacebookCookieSignInDto : FacebookSignInDto
{

    public bool RememberMe { get; set; } = true;


}