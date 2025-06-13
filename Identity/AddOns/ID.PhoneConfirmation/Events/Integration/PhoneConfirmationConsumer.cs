using ID.Application.AppAbs.Messaging;
using ID.GlobalSettings.Errors;
using ID.GlobalSettings.Setup.Options;
using ID.GlobalSettings.Utility;
using ID.IntegrationEvents.Abstractions;
using ID.IntegrationEvents.Events.Account.PhoneConfirmation;
using ID.GlobalSettings.Constants;
using LoggingHelpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StringHelpers;
using System.Diagnostics;

namespace ID.PhoneConfirmation.Events.Integration;
public class PhoneConfirmationConsumer(
    IIdSmsService smsService,
    ILogger<PhoneConfirmationConsumer> logger,
    IOptions<IdGlobalOptions> _globalOptionsProvider,
    IOptions<IdGlobalSetupOptions_CUSTOMER> _globalCusotmerOptionsProvider)
    : AEventHandler<PhoneConfirmationIntegrationEvent>
{
    private readonly IdGlobalOptions _globalOptions = _globalOptionsProvider.Value;
    private readonly IdGlobalSetupOptions_CUSTOMER _globalCustomerOptions = _globalCusotmerOptionsProvider.Value;

    //---------------------------------------//

    public override async Task HandleEventAsync(PhoneConfirmationIntegrationEvent data)
    {
        try
        {
            if (data.Phone.IsNullOrWhiteSpace())
                return;

            logger.LogError("{message}", $"PhoneConfirmationEvent: {data.Phone}: {data.Username}: {data.IsCustomerTeam}");
            Debug.WriteLine($"PhoneConfirmationEvent: {data.Phone}");

            var msg = data.IsCustomerTeam
                     ? CreateConfirmationMsgCustomer(data.UserId, data.Username, data.ConfirmationToken)
                     : CreateConfirmationMsgMntc(data.UserId, data.Username, data.ConfirmationToken);

            var sendResult = await smsService.SendMsgAsync(data.Phone, msg);

            if (!sendResult.Succeeded)
                logger.LogBasicResultFailure(sendResult, IdErrorEvents.Email.PhoneConfirmation);

            return;
        }
        catch (Exception e)
        {
            logger.LogException(e, IdErrorEvents.Email.PhoneConfirmation);
        }

    }


    //-------------------------------------//


    public string CreateConfirmationMsgCustomer(Guid userId, string username, string token)
    {
        string phoneConfirmationAddress = UrlBuilder.Combine(_globalCustomerOptions.CustomerAccountsUrl, IdGlobalConstants.PhoneRoutes.ConfirmPhone);
        string confirmLinkl = $"{phoneConfirmationAddress}?{IdGlobalConstants.PhoneRoutes.Params.UserId}={userId}&{IdGlobalConstants.PhoneRoutes.Params.ConfirmationToken}={token}";
        return PhoneConfirmationMsgTemplateGenerator.Generate(username, confirmLinkl, _globalOptions.ApplicationName);

    }


    //-------------------------------------//


    public string CreateConfirmationMsgMntc(Guid userId, string username, string token)
    {
        string phoneConfirmationAddress = UrlBuilder.Combine(_globalOptions.MntcAccountsUrl, IdGlobalConstants.PhoneRoutes.ConfirmPhone);
        string confirmLinkl = $"{phoneConfirmationAddress}?{IdGlobalConstants.PhoneRoutes.Params.UserId}={userId}&{IdGlobalConstants.PhoneRoutes.Params.ConfirmationToken}={token}";
        return PhoneConfirmationMsgTemplateGenerator.Generate(username, confirmLinkl, _globalOptions.ApplicationName);

    }


}//Cls
