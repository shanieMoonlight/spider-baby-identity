using ID.Email.Base.Abs;
using ID.Email.Base.AppAbs;
using ID.GlobalSettings.Errors;
using ID.IntegrationEvents.Abstractions;
using ID.IntegrationEvents.Events.Account.TwoFactor;
using LoggingHelpers;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace ID.Email.Base.EventListeners.TwoFactor;
public class TwoFactorAuthySetupRequestConsumer(
     IEmailDetailsTemplateGenerator emailDetailsTemplateGenerator,
     IIdEmailService emailService,
     ILogger<TwoFactorAuthySetupRequestConsumer> logger)
    : AEventHandler<TwoFactorAuthySetupRequestIntegrationEvent>
{
    public override async Task HandleEventAsync(TwoFactorAuthySetupRequestIntegrationEvent data)
    {
        try
        {
            Console.Beep();
            logger.LogError("{message}", $"TwoFactorRequestEvent: {data.Email}: {data.ManualQrCode}: {data.Name}");
            Debug.WriteLine($"TwoFactorRequestEvent: {data.Email}");


            var eDetails = await emailDetailsTemplateGenerator
                   .GenerateTwoFactorAuthTemplateAsync(
                        data.Name,
                        data.Email,
                        data.QrSrc,
                        data.ManualQrCode,
                        "Authy"
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

