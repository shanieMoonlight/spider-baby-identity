using ID.GlobalSettings.Setup;
using ID.Email.Base.AppAbs;
using ID.Email.Base.Setup;
using ID.IntegrationEvents.Abstractions;
using ID.IntegrationEvents.Events.Account.TwoFactor;
using LoggingHelpers;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using ID.Email.Base.Abs;
using ID.GlobalSettings.Errors;

namespace ID.Email.Base.EventListeners.TwoFactor;
public class TwoFactorGoogleSetupRequestConsumer(
     IEmailDetailsTemplateGenerator emailDetailsTemplateGenerator,
     IIdEmailService emailService,
     ILogger<TwoFactorGoogleSetupRequestConsumer> logger)
    : AEventHandler<TwoFactorGoogleSetupRequestIntegrationEvent>
{
    public override async Task HandleEventAsync(TwoFactorGoogleSetupRequestIntegrationEvent data)
    {
        try
        {
            Console.Beep();
            logger.LogError("{message}", $"TwoFactorRequestEvent: {data.Email}: {data.ManualQrCode}: {data.Name}");
            Debug.WriteLine($"TwoFactorRequestEvent: {data.Email}");


            var eDetails = await emailDetailsTemplateGenerator
                   .GenerateTwoFactorGoogleAuthTemplateAsync(
                    data.Name,
                    data.Email,
                    data.QrSrc,
                    data.ManualQrCode
               );

            var result = await emailService.SendEmailAsync(eDetails);

            if (!result.Succeeded)
                logger.LogBasicResultFailure(result, IdErrorEvents.Email.TwoFactor);

            return;
        }
        catch (Exception e)
        {
            logger.LogException(e, IdErrorEvents.Email.TwoFactor);
        }
    }


}//Cls

