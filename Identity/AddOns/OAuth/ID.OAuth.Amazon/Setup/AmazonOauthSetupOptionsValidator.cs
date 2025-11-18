using Microsoft.Extensions.Options;

namespace ID.OAuth.Amazon.Setup;

internal class AmazonOauthSetupOptionsValidator : IValidateOptions<IdOAuthAmazonOptions>
{
    public ValidateOptionsResult Validate(string? name, IdOAuthAmazonOptions options)
    {
        if (options is null)
            return ValidateOptionsResult.Fail("IdOAuthAmazonOptions is null.");

        var failures = new List<string>();

        if (string.IsNullOrWhiteSpace(options.ClientId))
            failures.Add("ClientId is required.");

        if (string.IsNullOrWhiteSpace(options.ClientSecret))
            failures.Add("ClientSecret is required.");

        if (options.RequestTimeoutSeconds <= 0)
            failures.Add("RequestTimeoutSeconds must be greater than zero.");

        //if (string.IsNullOrWhiteSpace(options.GraphApiVersion))
        //    failures.Add("GraphApiVersion is required.");

        //Don't validate because we'll fall back to GraphApi.BaseUrl if null or empty
        //if (string.IsNullOrWhiteSpace(options.GraphApiBaseUrl))
        //    failures.Add("GraphApiBaseUrl is required.");
        //else
        //{
        //    if (!Uri.TryCreate(options.GraphApiBaseUrl, UriKind.Absolute, out _))
        //        failures.Add("GraphApiBaseUrl must be a valid absolute URL.");
        //}

        return failures.Count > 0 ? ValidateOptionsResult.Fail(failures) : ValidateOptionsResult.Success;
    }
}
