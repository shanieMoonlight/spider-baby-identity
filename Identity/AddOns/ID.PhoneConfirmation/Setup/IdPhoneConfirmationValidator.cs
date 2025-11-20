using Microsoft.Extensions.Options;

namespace ID.PhoneConfirmation.Setup;

internal class IdPhoneConfirmationSetupOptionsValidator : IValidateOptions<IdPhoneConfirmationSetupOptions>
{
    public ValidateOptionsResult Validate(string? name, IdPhoneConfirmationSetupOptions options)
    {
        if (options is null)
            return ValidateOptionsResult.Fail($"{nameof(IdPhoneConfirmationSetupOptions)} is null.");


        // Add validation logic here in the future if needed.

        var failures = new List<string>();
        return failures.Count > 0 ? ValidateOptionsResult.Fail(failures) : ValidateOptionsResult.Success;
    }
}
