using ID.Email.Base.Abs;
using ID.Email.Base.AppAbs;
using ID.GlobalSettings.Constants;
using ID.GlobalSettings.Errors;
using ID.GlobalSettings.Setup.Options;
using ID.GlobalSettings.Utility;
using ID.IntegrationEvents.Abstractions;
using ID.IntegrationEvents.Events.Account.ForgotPwd;
using LoggingHelpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace ID.Email.Base.EventListeners.ForgotPwd;
public class ForgotPwdConsumer(
    IEmailDetailsTemplateGenerator emailDetailsTemplateGenerator,
    IIdEmailService emailService,
    IOptions<IdGlobalOptions> _globalOptionsProvider,
    IOptions<IdGlobalSetupOptions_CUSTOMER> _globalCusotmerOptionsProvider,
    ILogger<ForgotPwdConsumer> logger)
    : AEventHandler<ForgotPwdEmailRequestIntegrationEvent>
{
    private readonly IdGlobalOptions _globalOptions = _globalOptionsProvider.Value;
    private readonly IdGlobalSetupOptions_CUSTOMER _globalCustomerOptions = _globalCusotmerOptionsProvider.Value;

    //---------------------------------------//

    public override async Task HandleEventAsync(ForgotPwdEmailRequestIntegrationEvent data)
    {
        try
        {
            Console.Beep();
            logger.LogError("{message}", $"ForgotPasswordEvent: {data.Email}: {data.Phone}: {data.Name}: {data.IsCustomerTeam}");
            Debug.WriteLine($"ForgotPasswordEvent: {data.Email}");


            string accountsRoute = GetBaseUrl(data.IsCustomerTeam);
            string pwdResetTknAddress = UrlBuilder.Combine(accountsRoute, IdGlobalConstants.EmailRoutes.ResetPassword);
            string pwdResetTknUrl = $"{pwdResetTknAddress}?{IdGlobalConstants.EmailRoutes.Params.UserId}={data.UserId}&{IdGlobalConstants.EmailRoutes.Params.ResetToken}={data.ResetToken}";


            var eDetails = await emailDetailsTemplateGenerator
                   .GeneratePasswordResetTemplateAsync(
                      data.Name,
                      data.Email,
                      pwdResetTknUrl
                   );

            var result = await emailService.SendEmailAsync(eDetails);

            if (!result.Succeeded)
                logger.LogBasicResultFailure(result, IdErrorEvents.Email.ForgotPassword);

            return;
        }
        catch (Exception e)
        {
            logger.LogException(e, IdErrorEvents.Email.ForgotPassword);
        }
    }

    //---------------------------------------//

    private string GetBaseUrl(bool isCustomerTeam) =>
        isCustomerTeam
            ? _globalCustomerOptions.CustomerAccountsUrl
            : _globalOptions.MntcAccountsUrl;


}//Cls
