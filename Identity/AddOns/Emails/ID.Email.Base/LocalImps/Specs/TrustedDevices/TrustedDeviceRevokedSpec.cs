using ID.Email.Base.LocalAbs;
using ID.Email.Base.Models;
using ID.Email.Base.Setup;
using ID.GlobalSettings.Setup.Options;

namespace ID.Email.Base.LocalImps.Specs.TrustedDevices;

internal sealed class TrustedDeviceRevokedSpec(
    string toName,
    string toAddress,
    string deviceName,
    string userAgent,
    string ipAddress,
    string deviceMgmtUrl,
    string changePasswordUrl,
    DateTime dateRevoked)
    : IEmailSpec
{
    private const string _template_path = @"Assets\html-templates\TrustedDevices\IdTrustedDeviceRevoked.html";

    public async Task<IEmailDetails> BuildAsync(IdGlobalOptions globalOptions, ITemplateHelpers templateHelpers, IdEmailBaseOptions emailOptions)
    {
        var message = await templateHelpers.ReadAndReplaceTemplateAsync(_template_path, new Dictionary<string, string>
        {
            { EmailPlaceholders.PLACEHOLDER_USERNAME, toName },
            { EmailPlaceholders.PLACEHOLDER_DEVICE_UPDATE_DATETIME, dateRevoked.ToString("yyyy-MMM-dd HH:mm:ss") },
            { EmailPlaceholders.PLACEHOLDER_DEVICE_IPADDRESS, ipAddress },
            { EmailPlaceholders.PLACEHOLDER_DEVICE_USER_AGENT, userAgent },
            { EmailPlaceholders.PLACEHOLDER_DEVICE_NAME, deviceName },
            { EmailPlaceholders.PLACEHOLDER_DEVICE_MGMT_URL, deviceMgmtUrl },
            { EmailPlaceholders.PLACEHOLDER_CHANGE_PASSWORD_URL, changePasswordUrl },

        });



        return new EmailDetails(
            EmailType.HTML,
            message,
            $"Device Revoked - {globalOptions.ApplicationName}",
            toAddress,
            emailOptions.BccAddresses,
            emailOptions.FromAddress,
            emailOptions.FromName
        );
    }
}//Cls

