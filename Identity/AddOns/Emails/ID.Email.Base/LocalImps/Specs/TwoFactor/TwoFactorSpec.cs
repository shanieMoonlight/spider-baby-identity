using ID.Email.Base.LocalAbs;
using ID.Email.Base.Models;
using ID.Email.Base.Setup;
using ID.GlobalSettings.Setup.Options;

namespace ID.Email.Base.LocalImps.Specs.TwoFactor;

internal sealed class TwoFactorSpec(string toName, string toAddress, string subject, string verificationCode) : IEmailSpec
{
    private const string _template_path = @"Assets\html-templates\TwoFactor\IdTwoFactorLogin.html";

    public async Task<IEmailDetails> BuildAsync(IdGlobalOptions globalOptions, ITemplateHelpers templateHelpers, IdEmailBaseOptions emailOptions)
    {
        var message = await templateHelpers.ReadAndReplaceTemplateAsync(_template_path, new Dictionary<string, string>
        {
            { EmailPlaceholders.PLACEHOLDER_USERNAME, toName },
            { EmailPlaceholders.PLACEHOLDER_VERIFICATION_CODE, verificationCode }
        });

        return new EmailDetails(
            EmailType.HTML,
            message,
            subject,
            toAddress,
            emailOptions.BccAddresses,
            emailOptions.FromAddress,
            emailOptions.FromName
        );
    }
}
