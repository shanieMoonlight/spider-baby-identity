using ID.Email.Base.Abs;
using ID.Email.Base.AppAbs;
using ID.GlobalSettings.Constants;
using ID.GlobalSettings.Errors;
using ID.GlobalSettings.Setup.Options;
using ID.GlobalSettings.Utility;
using ID.IntegrationEvents.Abstractions;
using ID.IntegrationEvents.Events.Account.EmailConfirmation;
using LoggingHelpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyResults;
using System.Diagnostics;

namespace ID.Email.Base.EventListeners.EmailConfirmation;

/// <summary>
/// Handles email confirmation events by sending appropriate emails to customers or maintenance teams.
/// </summary>
public class EmailConfirmationConsumer(
            IEmailDetailsTemplateGenerator _emailDetailsTemplateGenerator,
            IIdEmailService _emailService,
            ILogger<EmailConfirmationConsumer> _logger,
            IOptions<IdGlobalOptions> _globalOptionsProvider,
            IOptions<IdGlobalSetupOptions_CUSTOMER> _globalCustomerOptionsProvider)
    : AEventHandler<EmailConfirmationIntegrationEvent>
{

    private readonly IdGlobalOptions _globalOptions = _globalOptionsProvider.Value;
    private readonly IdGlobalSetupOptions_CUSTOMER _globalCustomerOptions = _globalCustomerOptionsProvider.Value;

    //---------------------------------------//

    /// <summary>
    /// Handles the email confirmation event.
    /// </summary>
    /// <param name="data">The email confirmation event data.</param>
    public override async Task HandleEventAsync(EmailConfirmationIntegrationEvent data)
    {
        try
        {
            Console.Beep();
            Debug.WriteLine($"RegistrationEvent: {data.Email}");

            var result = data.IsCustomerTeam
                     ? await SendRegistrationEmailCustomer(data)
                     : await SendRegistrationEmailMntc(data);

            if (!result.Succeeded)
                _logger.LogBasicResultFailure(result, IdErrorEvents.Email.EmailConfirmation);

            return;
        }
        catch (Exception e)
        {
            _logger.LogException(e, IdErrorEvents.Email.EmailConfirmation);
        }
    }

    //------------------------------------//

    /// <summary>
    /// Sends a registration email to a customer.
    /// </summary>
    /// <param name="data">The email confirmation event data.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the basic result of the email sending operation.</returns>
    public async Task<BasicResult> SendRegistrationEmailCustomer(EmailConfirmationIntegrationEvent data)
    {
        string emailConfirmationAddress = UrlBuilder.Combine(_globalCustomerOptions.CustomerAccountsUrl, IdGlobalConstants.EmailRoutes.ConfirmEmail);
        string emailConfirmationUrl = $"{emailConfirmationAddress}?{IdGlobalConstants.EmailRoutes.Params.UserId}={data.UserId}&{IdGlobalConstants.EmailRoutes.Params.ConfirmationToken}={data.ConfirmationToken}";

        var eDetails = await _emailDetailsTemplateGenerator
           .GenerateEmailConfirmationCustomerTemplateAsync(
                  data.Name,
                  data.Email,
                  emailConfirmationUrl
           );

        return await _emailService.SendEmailAsync(eDetails);
    }

    //------------------------------------//

    /// <summary>
    /// Sends a registration email to a maintenance team.
    /// </summary>
    /// <param name="data">The email confirmation event data.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the basic result of the email sending operation.</returns>
    public async Task<BasicResult> SendRegistrationEmailMntc(EmailConfirmationIntegrationEvent data)
    {
        string emailConfirmationAddress = UrlBuilder.Combine(_globalOptions.MntcAccountsUrl, IdGlobalConstants.EmailRoutes.ConfirmEmail);
        string emailConfirmationUrl = $"{emailConfirmationAddress}?{IdGlobalConstants.EmailRoutes.Params.UserId}={data.UserId}&{IdGlobalConstants.EmailRoutes.Params.ConfirmationToken}={data.ConfirmationToken}";

        var eDetails = await _emailDetailsTemplateGenerator
           .GenerateEmailConfirmationMntcTemplateAsync(
                  data.Name,
                  data.Email,
                  emailConfirmationUrl
           );

        return await _emailService.SendEmailAsync(eDetails);
    }


}//Cls
