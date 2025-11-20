using Microsoft.Extensions.Options;

namespace ID.Email.SMTP.Setup;

internal class IdEmailSmtpOptionsValidator : IValidateOptions<IdEmailSmtpOptions>
{
    public ValidateOptionsResult Validate(string? name, IdEmailSmtpOptions options)
    {
        if (options is null)
            return ValidateOptionsResult.Fail("IdEmailSmtpOptions is null.");

        var failures = new List<string>();

        if (string.IsNullOrWhiteSpace(options.SmtpServerAddress))
            failures.Add("SmtpServerAddress is required.");

        if (options.SmtpPortNumber <= 0)
            failures.Add("SmtpPortNumber must be greater than 0.");

        if (string.IsNullOrWhiteSpace(options.SmtpUsernameOrEmail))
            failures.Add("SmtpUsernameOrEmail is required.");

        if (string.IsNullOrWhiteSpace(options.SmtpPassword))
            failures.Add("SmtpPassword is required.");

        return failures.Count > 0 ? ValidateOptionsResult.Fail(failures) : ValidateOptionsResult.Success;
    }
}
