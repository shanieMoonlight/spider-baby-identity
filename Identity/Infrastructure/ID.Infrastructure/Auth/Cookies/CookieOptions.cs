using ID.Infrastructure.Setup;
using StringHelpers;

namespace ID.Infrastructure.Auth.Cookies;
public class CookieOptions
{
    private string _cookieLoginPath = CookieDefaultValues.LOGIN_PATH.EnsureLeadingChar('/');
    /// <inheritdoc cref="IdInfrastructureSetupOptions.CookieLoginPath"/>
    public string CookieLoginPath
    {
        get => _cookieLoginPath;
        set => _cookieLoginPath = string.IsNullOrWhiteSpace(value)
            ? CookieDefaultValues.LOGIN_PATH.EnsureLeadingChar('/')
            : value.EnsureLeadingChar('/');
    }



    private string _cookieLogoutPath = CookieDefaultValues.LOGOUT_PATH.EnsureLeadingChar('/');
    /// <inheritdoc cref="IdInfrastructureSetupOptions.CookieLogoutPath"/>
    public string CookieLogoutPath
    {
        get => _cookieLogoutPath;
        set => _cookieLogoutPath = string.IsNullOrWhiteSpace(value)
            ? CookieDefaultValues.LOGOUT_PATH.EnsureLeadingChar('/')
            : value.EnsureLeadingChar('/');
    }



    private string _cookieAccessDeniedPath = CookieDefaultValues.ACCESS_DENIED_PATH.EnsureLeadingChar('/');
    /// <inheritdoc cref="IdInfrastructureSetupOptions.CookieAccessDeniedPath"/>
    public string CookieAccessDeniedPath
    {
        get => _cookieAccessDeniedPath;
        set => _cookieAccessDeniedPath = string.IsNullOrWhiteSpace(value)
            ? CookieDefaultValues.ACCESS_DENIED_PATH.EnsureLeadingChar('/')
            : value.EnsureLeadingChar('/');
    }



    private bool _cookieSlidingExpiration = CookieDefaultValues.SLIDING_EXPIRATION;
    /// <inheritdoc cref="IdInfrastructureSetupOptions.CookieSlidingExpiration"/>
    public bool CookieSlidingExpiration
    {
        get => _cookieSlidingExpiration;
        set => _cookieSlidingExpiration = value;
    }



    private TimeSpan _cookieExpireTimeSpan = CookieDefaultValues.EXPIRE_TIME_SPAN;
    /// <inheritdoc cref="IdInfrastructureSetupOptions.CookieExpireTimeSpan"/>
    public TimeSpan CookieExpireTimeSpan
    {
        get => _cookieExpireTimeSpan <= TimeSpan.Zero ? CookieDefaultValues.EXPIRE_TIME_SPAN : _cookieExpireTimeSpan;
        set => _cookieExpireTimeSpan = value;
    }


}//Cls
