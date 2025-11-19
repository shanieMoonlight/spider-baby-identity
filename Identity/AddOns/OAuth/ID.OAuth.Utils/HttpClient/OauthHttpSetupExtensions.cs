using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;

namespace ID.OAuth.Utils.HttpClient;
public static class OauthHttpSetupExtensions
{

    public static IHttpStandardResiliencePipelineBuilder AddMyIdOauthStandardResilienceHandler(this IHttpClientBuilder builder) =>
        builder.AddStandardResilienceHandler(options =>
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

}//Cls
