using ID.Email.Base.LocalAbs;
using ID.Email.Base.Models;
using ID.Email.Base.Setup;
using ID.GlobalSettings.Setup.Options;

namespace ID.Email.Base.LocalImps.Specs.TwoFactor;

internal sealed class TwoFactorAuthSpec(string toName, string toAddress, string qrSrc, string manualQrCode, string provider, string subject = "Two-Factor Setup") : IEmailSpec
{
    private const string _template_path = @"Assets\html-templates\TwoFactor\IdTwoFactorGoogleAuthSetup.html";

    public async Task<IEmailDetails> BuildAsync(IdGlobalOptions globalOptions, ITemplateHelpers templateHelpers, IdEmailBaseOptions emailOptions)
    {
        var message = await templateHelpers.ReadAndReplaceTemplateAsync(_template_path, new Dictionary<string, string>
        {
            { EmailPlaceholders.PLACEHOLDER_USERNAME, toName },
            { EmailPlaceholders.PLACEHOLDER_2_FACTOR_PROVIDER, provider },
            { EmailPlaceholders.PLACEHOLDER_MANUAL_QR_CODE, manualQrCode },
            { EmailPlaceholders.PLACEHOLDER_QR_IMG_SRC, qrSrc }
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
