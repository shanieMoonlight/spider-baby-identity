using ID.Email.Base.AppAbs;
using ID.Email.Base.LocalAbs;
using ID.Email.Base.LocalImps.Specs.TrustedDevices;
using ID.GlobalSettings.Errors;
using ID.GlobalSettings.Setup.Options;
using ID.IntegrationEvents.Abstractions;
using ID.IntegrationEvents.Events.Account.TrustedDevices;
using LoggingHelpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ID.Email.Base.EventListeners.TrustedDevices;
public class TrustedDeviceExpiredConsumer(
    IEmailDetailsTemplateGenerator emailDetailsTemplateGenerator,
    IIdEmailService emailService,
    IOptions<IdGlobalOptions> _globalOptionsProvider,
    IOptions<IdGlobalSetupOptions_CUSTOMER> _globalCusotmerOptionsProvider,
    ILogger<TrustedDeviceExpiredConsumer> logger)
    : AEventHandler<TrustedDeviceExpiredIntegrationEvent>
{
    private readonly IdGlobalOptions _globalOptions = _globalOptionsProvider.Value;
    private readonly IdGlobalSetupOptions_CUSTOMER _globalCustomerOptions = _globalCusotmerOptionsProvider.Value;

    //---------------------------------------//

    public override async Task HandleEventAsync(TrustedDeviceExpiredIntegrationEvent data)
    {
        try
        {

            string trustedDevicesUrl = TrustedDeviceConsumerUtils.GetTrustedDeviceMgmtUrl(
                data.IsCustomerTeam,
                _globalOptions,
                _globalCustomerOptions);

            var spec = new TrustedDeviceExpiredSpec(
                toName: data.UserName,
                toAddress: data.UserEmail,
                deviceName: data.DeviceName,
                userAgent: data.UserAgent,
                ipAddress: data.IpAddress,
                deviceMgmtUrl: trustedDevicesUrl,
                dateExpired: data.OccurredAtUtc);
            var eDetails = await emailDetailsTemplateGenerator.GenerateFromSpecAsync(spec);

            var result = await emailService.SendEmailAsync(eDetails);

            if (!result.Succeeded)
                logger.LogBasicResultFailure(result, IdErrorEvents.Email.TrustedDevices);

            return;
        }
        catch (Exception e)
        {
            logger.LogException(e, IdErrorEvents.Email.TrustedDevices);
        }
    }


}//Cls
