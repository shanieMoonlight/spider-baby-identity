using ID.OAuth.Facebook.Setup;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ID.OAuth.Facebook.HttpService;

/// <summary>
/// Configuration class for Facebook OAuth HTTP client with retry policies, timeouts, and circuit breaker.
/// Implements best practices for resilient HTTP communication with Facebook's Graph API.
/// </summary>
public static class FacebookHttpClientConfiguration
{
    /// <summary>
    /// Configures a named HttpClient for Facebook OAuth with resilience policies.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for method chaining</returns>
    /// <returns></returns>
    public static IServiceCollection AddFacebookOAuthHttpClient(this IServiceCollection services)
    {

        services.AddHttpClient<IFacebookHttpClient, FacebookHttpClient>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<IdOAuthFacebookOptions>>().Value;

            var baseUrl = string.IsNullOrWhiteSpace(options.GraphApiBaseUrl)
                 ? GraphApi.BaseUrl
                 : options.GraphApiBaseUrl;
            var apiVersion = options.GraphApiVersion ?? GraphApi.Version;

            var normalizedBase = baseUrl.TrimEnd('/');
            var versionSegment = string.IsNullOrWhiteSpace(apiVersion)
                ? string.Empty
                : apiVersion.Trim('/');

            // Configure base settings
            client.BaseAddress = new Uri(normalizedBase);
            client.Timeout = TimeSpan.FromSeconds(options.RequestTimeoutSeconds);

            // Add User-Agent header for better API rate limiting
            client.DefaultRequestHeaders.Add("User-Agent", "SpiderBaby-MyId-FacebookOAuth/1.0");
        })
        .AddStandardResilienceHandler(options =>
        {
            // Configure retry options
            options.Retry.MaxRetryAttempts = 3;
            options.Retry.Delay = TimeSpan.FromSeconds(1);

            // Configure circuit breaker options
            options.CircuitBreaker.FailureRatio = 0.5; // 50% failure rate
            options.CircuitBreaker.MinimumThroughput = 3; // At least 3 requests
            options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(30);

            // Configure timeout options
            options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(10);
            options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(30);
        });

        return services;
    }

}//Cls
