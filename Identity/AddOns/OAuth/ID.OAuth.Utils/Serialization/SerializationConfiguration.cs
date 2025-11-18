using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ID.OAuth.Utils.Serialization;

public static class SerializationConfiguration
{
    /// <summary>
    /// Configures JSON serialization options for OAuth.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection AddOAuthSerializationOptions(this IServiceCollection services)
    {

        // Register a shared JsonSerializerOptions for HttpClient deserialization  (Try*** in case this is called multiple times)
        services.TryAddSingleton(provider =>
        {
            var opts = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            // Register the Unix epoch converter for timestamps
            opts.Converters.Add(new UnixEpochSecondsJsonConverter());

            return opts;
        });

        return services;
    }

}//Cls
