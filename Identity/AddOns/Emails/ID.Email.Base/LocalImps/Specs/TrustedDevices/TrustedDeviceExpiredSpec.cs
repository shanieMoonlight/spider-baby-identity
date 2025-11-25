using ID.Email.Base.LocalAbs;
using ID.Email.Base.Models;
using ID.Email.Base.Setup;
using ID.GlobalSettings.Setup.Options;

namespace ID.Email.Base.LocalImps.Specs.TrustedDevices;

internal sealed class TrustedDeviceExpiredSpec(
    string toName,
    string toAddress,
    string deviceName,
    string userAgent,
    string ipAddress,
    string deviceMgmtUrl,
    DateTime dateExpired)
    : IEmailSpec
{
    private const string _template_path = @"Assets\html-templates\TrustedDevices\IdTrustedDeviceExpired.html";

    public async Task<IEmailDetails> BuildAsync(IdGlobalOptions globalOptions, ITemplateHelpers templateHelpers, IdEmailBaseOptions emailOptions)
    {
        var message = await templateHelpers.ReadAndReplaceTemplateAsync(_template_path, new Dictionary<string, string>
        {
            { EmailPlaceholders.PLACEHOLDER_USERNAME, toName },
            { EmailPlaceholders.PLACEHOLDER_DEVICE_UPDATE_DATETIME, dateExpired.ToString("yyyy-MMM-dd HH:mm:ss") },
            { EmailPlaceholders.PLACEHOLDER_DEVICE_IPADDRESS, ipAddress },
            { EmailPlaceholders.PLACEHOLDER_DEVICE_USER_AGENT, userAgent },
            { EmailPlaceholders.PLACEHOLDER_DEVICE_NAME, deviceName },
            { EmailPlaceholders.PLACEHOLDER_DEVICE_MGMT_URL, deviceMgmtUrl },

        });



        return new EmailDetails(
            EmailType.HTML,
            message,
            $"Device Expired - {globalOptions.ApplicationName}",
            toAddress,
            emailOptions.BccAddresses,
            emailOptions.FromAddress,
            emailOptions.FromName
        );
    }
}//Cls

