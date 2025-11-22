using ID.Domain.Entities.TrustedDevices;
using ID.Email.Base.LocalAbs;
using ID.Email.Base.Models;
using ID.Email.Base.Setup;
using ID.GlobalSettings.Setup.Options;

namespace ID.Email.Base.LocalImps.Specs.TrustedDevices;

internal sealed class TrustedDeviceAddedSpec(string toName, string toAddress, TrustedDevice device) : IEmailSpec
{
    private const string _template_path = @"Assets\html-templates\TrustedDevices\IdTrustedDeviceAdded.html";

    public async Task<IEmailDetails> BuildAsync(IdGlobalOptions globalOptions, ITemplateHelpers templateHelpers, IdEmailBaseOptions emailOptions)
    {
        var message = await templateHelpers.ReadAndReplaceTemplateAsync(_template_path, []);

        return new EmailDetails(
            EmailType.HTML,
            message,
            $"Device Added - {globalOptions.ApplicationName}",
            toAddress,
            emailOptions.BccAddresses,
            emailOptions.FromAddress,
            emailOptions.FromName
        );
    }
}

