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

        return failures.Count > 0 ? ValidateOptionsResult.Fail(failures) : ValidateOptionsResult.Success;
    }
}
