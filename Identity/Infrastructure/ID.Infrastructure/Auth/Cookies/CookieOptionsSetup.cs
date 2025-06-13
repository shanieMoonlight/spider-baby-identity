using ID.Domain.Utility.Messages;
using ID.Infrastructure.Setup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ID.Infrastructure.Auth.Cookies;
internal static class CookieOptionsSetup
{

    internal static IServiceCollection ConfigureCookieOptions(this IServiceCollection services, IdInfrastructureSetupOptions setupOptions)
    {
        services.Configure<CookieOptions>(cookieOptions =>
        {
            cookieOptions.CookieExpireTimeSpan = setupOptions.CookieExpireTimeSpan ?? CookieDefaultValues.EXPIRE_TIME_SPAN;
            cookieOptions.CookieSlidingExpiration = setupOptions.CookieSlidingExpiration ?? CookieDefaultValues.SLIDING_EXPIRATION;
            cookieOptions.CookieLogoutPath = setupOptions.CookieLogoutPath ?? CookieDefaultValues.LOGOUT_PATH;
            cookieOptions.CookieLoginPath = setupOptions.CookieLoginPath ?? CookieDefaultValues.LOGIN_PATH;
            cookieOptions.CookieAccessDeniedPath = setupOptions.CookieAccessDeniedPath ?? CookieDefaultValues.ACCESS_DENIED_PATH;
        });

        return services;
    }


    //-------------------------------------//

    internal static IServiceCollection ConfigureCookieOptions(this IServiceCollection services, IConfiguration configuration, string? sectionName = null)
    {
        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration), IDMsgs.Error.Setup.MISSING_CONFIGURATION);        // If sectionName provided, get that section; otherwise assume configuration is already the JWT section
        var configSection = string.IsNullOrWhiteSpace(sectionName)
            ? configuration
            : configuration.GetSection(sectionName);

        services.Configure<CookieOptions>(configSection);

        return services;
    }



}//Cls
