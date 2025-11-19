using ID.OAuth.Utils.Serialization;
using ID.OAuth.Utils.Services.Abs;
using ID.OAuth.Utils.Services.Imps;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ID.OAuth.Utils.Setup;

/// <summary>
/// Extension methods for configuring OAuth Utils services.
/// </summary>
public static class OAuthUtilsSetupExtensions
{

    /// <summary>
    /// Adds MyId OAuth services.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection AddMyIdOAuthUtils(this IServiceCollection services)
    {

        services.TryAddScoped<IOAuthHttpClientUtils, OAuthHttpClientUtils>();
        services.AddOAuthSerializationOptions();

        return services;

    }

}//Cls
