using ID.OAuth.Facebook.HttpService;

namespace ID.OAuth.Facebook.Setup;

public class IdOAuthFacebookOptions
{
    /// <summary>
    /// Facebook App ID from your Facebook App configuration
    /// </summary>
    public string AppId { get; set; } = string.Empty;

    /// <summary>
    /// Facebook App Secret from your Facebook App configuration
    /// </summary>
    public string AppSecret { get; set; } = string.Empty;

    /// <summary>
    /// Facebook Graph API version to use for API calls
    /// </summary>
    public string GraphApiVersion { get; set; } = GraphApi.Version;

    /// <summary>
    /// Base URL for Facebook Graph API calls
    /// </summary>
    public string GraphApiBaseUrl { get; set; } = GraphApi.BaseUrl;

    /// <summary>
    /// Timeout in seconds for HTTP requests to Facebook API
    /// </summary>
    public int RequestTimeoutSeconds { get; set; } = 30;

    //// Optional: callback path used by the ASP.NET Core Facebook middleware
    //public string CallbackPath { get; set; } = "/signin-facebook";

    //// Optional additional scopes requested from Facebook
    //public string[]? Scopes { get; set; }
}
