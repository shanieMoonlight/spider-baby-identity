using FluentValidation;
using ID.OAuth.Facebook.HttpService;
using ID.OAuth.Facebook.Services;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ID.OAuth.Utils.Setup;

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
        // Configure Facebook OAuth options and validate on start
        services.AddOptionsWithValidateOnStart<IdOAuthFacebookOptions, FbOauthSetupOptionsValidator>()
                .Bind(configuration.GetSection(sectionName));

        return services.AddFacebookOAuthDI();

    }

    //----------------------//

    /// <summary>
    /// Adds Facebook OAuth services with explicit options configuration.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="config">Action to configure Facebook OAuth options</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection AddMyIdFacebookOAuth(
        this IServiceCollection services,
        Action<IdOAuthFacebookOptions> config)
    {
        // Configure Facebook OAuth options and validate on start
        services
            .Configure(config);

        services.AddOptionsWithValidateOnStart<IdOAuthFacebookOptions, FbOauthSetupOptionsValidator>();

        // Register Facebook OAuth services
        return services.AddFacebookOAuthDI();

    }

    //----------------------//

    public static IServiceCollection AddFacebookOAuthDI(this IServiceCollection services)
    {
        // Configure named HttpClient with resilience policies
        services.AddMyIdOAuthUtils();

        services.AddFacebookOAuthHttpClient();

        // Register Facebook OAuth services
        services.AddFacebookOAuthServices();

        var assembly = typeof(IdFacebookOAuthAssemblyReference).Assembly;

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

}//Cls
