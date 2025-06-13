using ID.Email.Base.AppAbs;
using ID.IntegrationEvents.Abstractions;
using ID.IntegrationEvents.Events.Account.TwoFactor;
using LoggingHelpers;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using ID.Email.Base.Abs;
using ID.GlobalSettings.Errors;

namespace ID.Email.Base.EventListeners.TwoFactor;
public class TwoFactorEmailRequestConsumer(
     IEmailDetailsTemplateGenerator emailDetailsTemplateGenerator,
     IIdEmailService emailService,
     ILogger<TwoFactorEmailRequestConsumer> logger)
    : AEventHandler<TwoFactorEmailRequestIntegrationEvent>
{
    public override async Task HandleEventAsync(TwoFactorEmailRequestIntegrationEvent data)
    {
        try
        {
            Console.Beep();
            logger.LogError("{message}", $"TwoFactorRequestEvent: {data.Email}: {data.Phone}: {data.Name}");
            Debug.WriteLine($"TwoFactorRequestEvent: {data.Email}");


            var eDetails = await emailDetailsTemplateGenerator
                   .GenerateTwoFactorTemplateAsync(
                    data.Name,
                    data.Email,
                    "Verification Code",
                    data.VerificationCode
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

