namespace ID.OAuth.Facebook.Models;

public sealed class IdOAuthFacebookOptions
{
    // AppId (Facebook "App ID")
    public string AppId { get; set; } = string.Empty;

    // AppSecret (Facebook "App Secret")
    public string AppSecret { get; set; } = string.Empty;

    // Optional: callback path used by the ASP.NET Core Facebook middleware
    public string CallbackPath { get; set; } = "/signin-facebook";

    // Optional additional scopes requested from Facebook
    public string[]? Scopes { get; set; }
}
