using Microsoft.Extensions.Options;

namespace ID.Email.SG.Setup;

internal class IdEmailSgOptionsValidator : IValidateOptions<IdEmailSgOptions>
{
    public ValidateOptionsResult Validate(string? name, IdEmailSgOptions options)
    {
        if (options is null)
            return ValidateOptionsResult.Fail("IdEmailSgOptions is null.");

        var failures = new List<string>();

        if (string.IsNullOrWhiteSpace(options.ApiKey))
            failures.Add("ApiKey is required.");

        return failures.Count > 0 ? ValidateOptionsResult.Fail(failures) : ValidateOptionsResult.Success;
    }
}
