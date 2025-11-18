using ID.OAuth.Amazon.HttpService.Abs;
using ID.OAuth.Amazon.HttpService.Imps;
using ID.OAuth.Amazon.Setup;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ID.OAuth.Utils.HttpClient;

namespace ID.OAuth.Amazon.HttpService;

public static class AmazonHttpClientConfiguration
{
    public static IServiceCollection AddAmazonOAuthHttpClient(this IServiceCollection services)
    {

        services.AddHttpClient<IAmazonHttpClient, AmazonHttpClient>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<IdOAuthAmazonOptions>>().Value;

            var baseUrl = string.IsNullOrWhiteSpace(options.ApiBaseUrl)
                ? AmazonApi.BaseUrl
                : options.ApiBaseUrl;

            //var apiVersion = options.GraphApiVersion ?? GraphApi.Version;

            var normalizedBase = baseUrl.TrimEnd('/');
            //var versionSegment = string.IsNullOrWhiteSpace(apiVersion)
            //    ? string.Empty
            //    : apiVersion.Trim('/');

            // Configure base settings
            client.BaseAddress = new Uri(normalizedBase);
            client.Timeout = TimeSpan.FromSeconds(options.RequestTimeoutSeconds);

            // Add User-Agent header for better API rate limiting
            client.DefaultRequestHeaders.Add("User-Agent", "SpiderBaby-MyId-AmazonOAuth/1.0");
        })
        .AddMyIdOauthStandardResilienceHandler();

        return services;
    }
}
