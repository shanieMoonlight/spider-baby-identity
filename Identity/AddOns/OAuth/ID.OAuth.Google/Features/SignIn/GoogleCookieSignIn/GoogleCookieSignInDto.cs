namespace ID.OAuth.Google.Features.SignIn.GoogleCookieSignIn;

public sealed class GoogleCookieSignInDto : GoogleSignInDto
{

    public bool RememberMe { get; set; } = true;


}