using ID.Email.Base.AppAbs;
using ID.Email.Base.LocalAbs;
using ID.Email.Base.LocalImps.Specs.Subscriptions;
using ID.GlobalSettings.Errors;
using ID.IntegrationEvents.Abstractions;
using ID.IntegrationEvents.Events.Account.Subscriptions;
using LoggingHelpers;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace ID.Email.Base.EventListeners.Subscriptions;
public class SubscriptionsPausedConsumer(
     IEmailDetailsTemplateGenerator emailDetailsTemplateGenerator,
     IIdEmailService emailService,
     ILogger<SubscriptionsPausedConsumer> logger)
    : AEventHandler<SubscriptionsPausedIntegrationEvent>
{
    public override async Task HandleEventAsync(SubscriptionsPausedIntegrationEvent data)
    {
        try
        {
            Console.Beep();
            Debug.WriteLine($"{nameof(SubscriptionsPausedIntegrationEvent)}: {data.ToName}, {data.SubscriptionPlanName}");


            var spec = new SubscriptionPausedSpec(data.ToName, data.Email, data.SubscriptionPlanName);
            var eDetails = await emailDetailsTemplateGenerator.GenerateFromSpecAsync(spec);

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

