using ID.OAuth.Facebook.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ID.OAuth.Facebook.Setup;

/// <summary>
/// Extension methods for configuring Facebook OAuth services.
/// </summary>
public static class FacebookOAuthServiceCollectionExtensions
{
    /// <summary>
    /// Adds Facebook OAuth services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">Configuration containing Facebook OAuth settings</param>
    /// <param name="sectionName">Configuration section name (default: "FacebookOAuth")</param>
    /// <returns>The service collection for method chaining</returns>    
    public static IServiceCollection AddFacebookOAuth(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = "FacebookOAuth")
    {
        // Configure Facebook OAuth options
        services.Configure<IdOAuthFacebookOptions>(
            configuration.GetSection(sectionName));

        // Register Facebook OAuth services
        services.AddScoped<IFacebookTokenVerifier, FacebookTokenVerifier>();

        // Configure named HttpClient with resilience policies
        services.AddFacebookOAuthHttpClient();

        return services;
    }

    //----------------------//

    /// <summary>
    /// Adds Facebook OAuth services with explicit options configuration.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configureOptions">Action to configure Facebook OAuth options</param>
    /// <returns>The service collection for method chaining</returns>   

    public static IServiceCollection AddFacebookOAuth(
         this IServiceCollection services,
         Action<IdOAuthFacebookOptions> configureOptions)
    {
        // Configure Facebook OAuth options
        services.Configure(configureOptions);

        // Register Facebook OAuth services
        services.AddScoped<IFacebookTokenVerifier, FacebookTokenVerifier>();

        // Configure named HttpClient with resilience policies
        services.AddFacebookOAuthHttpClient();

        return services;
    }

}//Cls
