using FluentValidation;
using ID.Domain.Entities.AppUsers;
using ID.OAuth.Facebook.HttpService;
using ID.OAuth.Facebook.Services;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ID.OAuth.Facebook.Setup;

/// <summary>
/// Extension methods for configuring Facebook OAuth services.
/// </summary>
public static class FacebookOAuthSetupExtensions
{
    /// <summary>
    /// Adds Facebook OAuth services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">Configuration containing Facebook OAuth settings</param>
    /// <param name="sectionName">Configuration section name (default: "FacebookOAuth")</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection AddMyIdFacebookOAuth(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = "FacebookOAuth")
    {
        // Configure Facebook OAuth options
        services.Configure<IdOAuthFacebookOptions>(
            configuration.GetSection(sectionName));


        return services.AddFacebookOAuthDI();

    }

    //----------------------//

    /// <summary>
    /// Adds Facebook OAuth services with explicit options configuration.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configureOptions">Action to configure Facebook OAuth options</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection AddMyIdFacebookOAuth(
        this IServiceCollection services,
        Action<IdOAuthFacebookOptions> configureOptions)
    {
        // Configure Facebook OAuth options
        services.Configure(configureOptions);

        // Register Facebook OAuth services
        return services.AddFacebookOAuthDI();

    }

    public static IServiceCollection AddFacebookOAuthDI(this IServiceCollection services)
    {

        // Configure named HttpClient with resilience policies
        services.AddFacebookOAuthHttpClient();

        // Register Facebook OAuth services
        services.AddScoped<IFacebookAuthenticationService, FacebookAuthenticationService>();
        services.AddScoped<IFacebookClientUtilities, FacebookClientUtilities>();
        services.AddScoped<IFindOrCreateService<AppUser>, FindOrCreateService<AppUser>>();

        var assembly = typeof(IdFacebookOAuthAssemblyReference).Assembly;
        //IdOAuthFacebookOptionsSetup.ConfigureIdOAuthFacebookOptions(services, setupOptions ?? new IdOAuthFacebookOptions());

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(assembly);
        });

        // Add FluentValidation validators from the Customers assembly
        services.AddValidatorsFromAssembly(assembly);

        services.AddControllers()
            .PartManager.ApplicationParts.Add(new AssemblyPart(typeof(IdFacebookOAuthAssemblyReference).Assembly));

        return services;
    }

}
