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

namespace ID.Email.Base.EventListeners.EmailConfirmation;

/// <summary>
/// Handles the event for email confirmation requiring a password.
/// </summary>
public class EmailConfirmationRequiringPasswordConsumer(
    IEmailDetailsTemplateGenerator emailDetailsTemplateGenerator,
    IOptions<IdGlobalOptions> _globalOptionsProvider,
    IOptions<IdGlobalSetupOptions_CUSTOMER> _globalCustomerOptionsProvider,
    IIdEmailService emailService,
    ILogger<EmailConfirmationRequiringPasswordConsumer> logger)
    : AEventHandler<EmailConfirmationRequiringPasswordIntegrationEvent>//, IConsumer<EmailConfirmationRequiringPasswordEvent>
{

    private readonly IdGlobalOptions _globalOptions = _globalOptionsProvider.Value;
    private readonly IdGlobalSetupOptions_CUSTOMER _globalCustomerOptions = _globalCustomerOptionsProvider.Value;

    //---------------------------------------//

    /// <summary>
    /// Handles the email confirmation event.
    /// </summary>
    /// <param name="data">The event data containing email confirmation details.</param>
    public override async Task HandleEventAsync(EmailConfirmationRequiringPasswordIntegrationEvent data)
    {
        try
        {
            var result = data.IsCustomerTeam
                     ? await SendRegistrationEmailCustomer(data)
                     : await SendRegistrationEmailMntc(data);


            if (!result.Succeeded)
                logger.LogBasicResultFailure(result, IdErrorEvents.Email.EmailConfirmation);

            return;
        }
        catch (Exception e)
        {
            logger.LogException(e, IdErrorEvents.Email.EmailConfirmation);
        }
    }


    //---------------------------------------//

    /// <summary>
    /// Sends a registration email to a customer.
    /// </summary>
    /// <param name="data">The event data containing email confirmation details.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the result of the email sending operation.</returns>
    public async Task<BasicResult> SendRegistrationEmailCustomer(EmailConfirmationRequiringPasswordIntegrationEvent data)
    {

        //string emailConfirmationAddress = $"{EmailSgSettings.BaseUrl}/{EmailSgSettings.CustomerAccountsRoute}/{IdDomainConstants.EmailRoutes.ConfirmEmailWithPassword}";
        string emailConfirmationAddress = UrlBuilder.Combine(_globalCustomerOptions.CustomerAccountsUrl, IdGlobalConstants.EmailRoutes.ConfirmEmailWithPassword);
        string emailConfirmationUrl = $"{emailConfirmationAddress}?{IdGlobalConstants.EmailRoutes.Params.UserId}={data.UserId}&{IdGlobalConstants.EmailRoutes.Params.ConfirmationToken}={data.ConfirmationToken}";

        var eDetails = await emailDetailsTemplateGenerator
           .GenerateEmailConfirmationCustomerTemplateAsync(
                  data.Name,
                  data.Email,
                  emailConfirmationUrl
           );

        return await emailService.SendEmailAsync(eDetails);

    }

    //---------------------------------------//

    /// <summary>
    /// Sends a registration email to a maintenance team member.
    /// </summary>
    /// <param name="data">The event data containing email confirmation details.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the result of the email sending operation.</returns>
    public async Task<BasicResult> SendRegistrationEmailMntc(EmailConfirmationRequiringPasswordIntegrationEvent data)
    {

        //string emailConfirmationAddress = $"{EmailSgSettings.BaseUrl}/{EmailSgSettings.MntcAccountsRoute}/{IdDomainConstants.EmailRoutes.ConfirmEmailWithPassword}";
        string emailConfirmationAddress = UrlBuilder.Combine(_globalOptions.MntcAccountsUrl, IdGlobalConstants.EmailRoutes.ConfirmEmailWithPassword);
        string emailConfirmationUrl = $"{emailConfirmationAddress}?{IdGlobalConstants.EmailRoutes.Params.UserId}={data.UserId}&{IdGlobalConstants.EmailRoutes.Params.ConfirmationToken}={data.ConfirmationToken}";

        var eDetails = await emailDetailsTemplateGenerator
           .GenerateEmailConfirmationMntcTemplateAsync(
                  data.Name,
                  data.Email,
                  emailConfirmationUrl
           );

        return await emailService.SendEmailAsync(eDetails);

    }


}//Cls


