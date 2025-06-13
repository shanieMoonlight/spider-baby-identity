namespace ID.Infrastructure.Auth.Cookies;

internal class CookieDefaultValues
{
    /// <summary>
    /// "/Account/Signin"
    /// </summary>
    public const string LOGIN_PATH = "/Account/Signin";

    /// <summary>
    /// "/Account/Logout"
    /// </summary>
    public const string LOGOUT_PATH = "/Account/Logout";

    /// <summary>
    ///  "/Account/AccessDenied"
    /// </summary>
    public const string ACCESS_DENIED_PATH = "/Account/AccessDenied";

    /// <summary>
    /// true
    /// </summary>
    public const bool SLIDING_EXPIRATION = true;

    /// <summary>
    /// TimeSpan.FromMinutes(60) - 1 Hour
    /// </summary>
    public static readonly TimeSpan EXPIRE_TIME_SPAN = TimeSpan.FromMinutes(60);

}
