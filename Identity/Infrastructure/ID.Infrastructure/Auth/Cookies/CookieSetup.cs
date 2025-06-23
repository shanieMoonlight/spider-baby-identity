using ID.Application.AppAbs.SignIn;
using ID.Domain.Entities.AppUsers;
using ID.Infrastructure.Auth.Cookies.Events;
using ID.Infrastructure.Auth.Cookies.Services;
using ID.Infrastructure.Setup;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace ID.Infrastructure.Auth.Cookies;
internal static class CookieSetup
{

    internal static IServiceCollection AddMyIdCookies<TUser>(
        this IServiceCollection services, IdInfrastructureSetupOptions setupOptions)
        where TUser : AppUser
    {

        services.ConfigureCookieOptions(setupOptions);

        services.AddCookieServices<TUser>();

        return services;
    }


    //-------------------------------------//


    internal static IServiceCollection AddMyIdCookies<TUser>(
        this IServiceCollection services, IConfiguration configuration, string? sectionName = null)
        where TUser : AppUser
    {

        services.ConfigureCookieOptions(configuration, sectionName);

        services.AddCookieServices<TUser>();

        return services;
    }


    //-------------------------------------//

    internal static AuthenticationBuilder UseCookieAuth(this AuthenticationBuilder authBuilder)
    {
        // ✅ Register a configuration provider that will resolve services LATER
        authBuilder.Services.AddSingleton<IConfigureOptions<CookieAuthenticationOptions>>(serviceProvider =>
        {
            return new ConfigureNamedOptions<CookieAuthenticationOptions>(
                CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    // Service resolution happens WHEN OPTIONS ARE REQUESTED
                    // Will be called once then the result wil  be cached
                    var cookieOptions = serviceProvider.GetRequiredService<IOptions<CookieOptions>>().Value;
                    var env = serviceProvider.GetRequiredService<IWebHostEnvironment>();

                    options.LoginPath = cookieOptions.CookieLoginPath;
                    options.LogoutPath = cookieOptions.CookieLogoutPath;
                    options.AccessDeniedPath = cookieOptions.CookieAccessDeniedPath;
                    options.SlidingExpiration = cookieOptions.CookieSlidingExpiration;
                    options.ExpireTimeSpan = cookieOptions.CookieExpireTimeSpan;

                    if (env.IsDevelopment())
                        options.Events = new DebugCookieEvents();

                });
        });

        return authBuilder.AddCookie(); // No configuration here
    }

    //-----------------------//


    private static IServiceCollection AddCookieServices<TUser>(this IServiceCollection services) where TUser : AppUser
    {
        services.TryAddScoped<ICookieAuthService<TUser>, CookieAuthService<TUser>>();

        return services;
    }



}//Cls
