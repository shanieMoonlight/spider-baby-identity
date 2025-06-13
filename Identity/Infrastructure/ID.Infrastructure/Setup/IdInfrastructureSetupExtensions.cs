using ID.Application.AppAbs.ExtraClaims;
using ID.Application.AppAbs.FromApp;
using ID.Application.AppAbs.Messaging;
using ID.Application.AppAbs.MFA.AuthenticatorApps;
using ID.Application.AppAbs.Setup;
using ID.Domain.Abstractions.PasswordValidation;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Infrastructure.Auth;
using ID.Infrastructure.Auth.Cookies;
using ID.Infrastructure.Auth.JWT.Setup;
using ID.Infrastructure.Claims.Services;
using ID.Infrastructure.Claims.Services.Abs;
using ID.Infrastructure.Claims.Services.Imps;
using ID.Infrastructure.DomainServices;
using ID.Infrastructure.Jobs;
using ID.Infrastructure.Persistance.EF.Setup;
using ID.Infrastructure.Persistance.EF.Setup.Options;
using ID.Infrastructure.Services.FromApp;
using ID.Infrastructure.Services.Google;
using ID.Infrastructure.Services.Initialization;
using ID.Infrastructure.Services.Initialization.UsersAndRoles;
using ID.Infrastructure.Services.Messaging;
using ID.Infrastructure.Services.PasswordValidation;
using ID.Infrastructure.Setup.Options;
using ID.Infrastructure.Setup.Passwords;
using ID.Infrastructure.Setup.SignIn;
using ID.Infrastructure.TokenServices;
using ID.Infrastructure.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace ID.Infrastructure.Setup;

public static class IdInfrastructureSetupExtensions
{
    /// <summary>
    /// Setup MyIdentity
    /// </summary>
    public static AuthenticationBuilder AddIdInfrastructure<TExtraClaimsGenerator>(this IServiceCollection services, IdInfrastructureSetupOptions setupOptions)
        where TExtraClaimsGenerator : class, IExtraClaimsGenerator
        => services.Configure<TExtraClaimsGenerator>(setupOptions);

    //-----------------------//

    /// <summary>
    /// Configures Middleware and Exception Handling for IdInfrastructure
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseMyIdInfrastructure(this IApplicationBuilder app, TeamType minTypeDashboardAccess) =>
        app.UseMyIdJobs(minTypeDashboardAccess);


    //-----------------------//


    private static AuthenticationBuilder Configure<TExtraClaimsGenerator>(
        this IServiceCollection services, IdInfrastructureSetupOptions setupOptions)
        where TExtraClaimsGenerator : class, IExtraClaimsGenerator
    {
        ArgumentNullException.ThrowIfNull(setupOptions.ConnectionString);  //This is fatal we must throw

        //First, configure settings!!!
        services.ConfigureSettings<AppUser>(setupOptions);

        var idBuilder = services.AddIdentity();

        var authBuilder = services.AddAuth(setupOptions);

        services.ConfigureDependencyInjection<AppUser>(setupOptions)
            .AddPersistenceEf(setupOptions, idBuilder);

        services.SetupJobs(setupOptions);

        services.SetupAuthChallengers(setupOptions);

        services.TryAddTransient<IExtraClaimsGenerator, TExtraClaimsGenerator>();

        return authBuilder;

    }

    //-----------------------//    

    /// <summary>
    /// Add my identity and migrates any changes
    /// </summary>
    /// <param name="services">Collection of services</param>
    private static IdentityBuilder AddIdentity(this IServiceCollection services)
    {
        return services.AddIdentity<AppUser, AppRole>()
            .AddDefaultTokenProviders()
            .AddErrorDescriber<CustomIdentityErrorDescriber>();



        //Make sure database is up to date.
        //if (!MyEnvironment.IsDevelopment())
        //services.EfMigrate<MyIdentityContext>();
    }

    //-----------------------//

    /// <summary>
    /// Configures all DI
    /// </summary>
    /// <param name="services">Collection of services</param>
    private static IServiceCollection ConfigureDependencyInjection<TUser>(this IServiceCollection services, IdInfrastructureSetupOptions setupOptions) where TUser : AppUser
    {
        services.AddDomainServices<TUser>();

        //Should be replaced by Twilio or something like that. The user will choose
        services.TryAddTransient<IIdSmsService, IdFallbackMsgService>();
        services.TryAddTransient<IIdWhatsAppService, IdFallbackMsgService>();

        services.TryAddSingleton<IIsFromMobileApp, FromMobileApp>();

        services.TryAddTransient<IUserAndRoleDataInitializer, UserAndRoleDataInitializer>();
        services.TryAddTransient<IIdentityInitializationService, InitializationService>();


        //Use this as default Authenticator App Service. Might be replaced later.
        services.TryAddTransient<IAuthenticatorAppService, GoogleAuthService>();


        services.TryAddTransient<IIdPasswordValidator, IdPasswordValidator>();



        services
            .AddHttpContextAccessor()
            .AddTokenBasedServices<AppUser>(setupOptions.UseDbTokenProvider ?? InfrastructureDefaultValues.USE_DB_TOKEN_PROVIDER);

        return services;
    }

    //-----------------------//

    /// <summary>
    /// Setup Authentication
    /// </summary>
    /// <param name="services">Collection of services</param>
    /// <param name="setupOptions">Setup Config</param>
    private static AuthenticationBuilder AddAuth(this IServiceCollection services, IdInfrastructureSetupOptions setupOptions)
    {


        AuthenticationBuilder authBuilder = services.AddAuthentication(options =>
        {
            // Identity made Cookie authentication the default.
            // However, we want JWT Bearer Auth to be the default.
            //options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

            options.DefaultAuthenticateScheme = CustomAuthenticationHandler.SchemeName;
            options.DefaultChallengeScheme = CustomAuthenticationHandler.SchemeName;
            options.DefaultScheme = CustomAuthenticationHandler.SchemeName;



            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;

        })
        .AddScheme<AuthenticationSchemeOptions, CustomAuthenticationHandler>(CustomAuthenticationHandler.SchemeName, options => { })
        .UseJwtAuth()
        .UseCookieAuth();

        return authBuilder;
    }

    //-----------------------//

    /// <summary>
    /// Apply any user settings
    /// </summary>
    private static void ConfigureSettings<TUser>(this IServiceCollection services, IdInfrastructureSetupOptions setupOptions) where TUser : AppUser
    {
        if (setupOptions == null)
            throw new ArgumentNullException(nameof(setupOptions), "You must at least supply the database connection string.");

        services
            .AddMyIdClaims()
            .AddMyIdJwt(setupOptions)
            .AddMyIdCookies<TUser>(setupOptions)
            .ConfigurePasswordOptions(setupOptions)
            .ConfigureSignInOptions(setupOptions)
            .ConfigureInfrastructureOptions(setupOptions)
            .ConfigureIdentityOptions();

    }

    //-----------------------//

    /// <summary>
    /// Configures Identity options using DI-based options classes
    /// </summary>
    private static IServiceCollection ConfigureIdentityOptions(this IServiceCollection services)
    {
        services.AddSingleton<IConfigureOptions<IdentityOptions>>(serviceProvider =>
        {
            return new ConfigureNamedOptions<IdentityOptions>(null, options =>
            {
                var passwordOptions = serviceProvider.GetRequiredService<IOptions<IdPasswordOptions>>().Value;
                var signInOptions = serviceProvider.GetRequiredService<IOptions<IdSignInOptions>>().Value;

                // Configure password options
                options.Password.RequireDigit = passwordOptions.RequireDigit;
                options.Password.RequireLowercase = passwordOptions.RequireLowercase;
                options.Password.RequireUppercase = passwordOptions.RequireUppercase;
                options.Password.RequireNonAlphanumeric = passwordOptions.RequireNonAlphanumeric;
                options.Password.RequiredLength = passwordOptions.RequiredLength;
                options.Password.RequiredUniqueChars = passwordOptions.RequiredUniqueChars;

                // Configure sign-in options
                options.SignIn.RequireConfirmedEmail = signInOptions.RequireConfirmedEmail;
                options.SignIn.RequireConfirmedPhoneNumber = signInOptions.RequireConfirmedPhoneNumber;

                // Set other common options
                options.User.RequireUniqueEmail = true;
            });
        });

        return services;
    }


}//Cls

