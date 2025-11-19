namespace ID.OAuth.Amazon.Features.SignIn.AmazonCookieSignIn;

public sealed class AmazonCookieSignInDto : ID.OAuth.Amazon.Features.SignIn.AmazonSignInDto
{
    public bool RememberMe { get; set; }
}
