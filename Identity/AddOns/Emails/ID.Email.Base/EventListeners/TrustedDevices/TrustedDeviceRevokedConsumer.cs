using ID.Email.Base.AppAbs;
using ID.Email.Base.LocalAbs;
using ID.Email.Base.LocalImps.Specs;
using ID.GlobalSettings.Constants;
using ID.GlobalSettings.Errors;
using ID.GlobalSettings.Setup.Options;
using ID.GlobalSettings.Utility;
using ID.IntegrationEvents.Abstractions;
using ID.IntegrationEvents.Events.Account.ForgotPwd;
using ID.IntegrationEvents.Events.Account.TrustedDevices;
using LoggingHelpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace ID.Email.Base.EventListeners.TrustedDevices;
public class TrustedDeviceRevokedConsumer(
    IEmailDetailsTemplateGenerator emailDetailsTemplateGenerator,
    IIdEmailService emailService,
    IOptions<IdGlobalOptions> _globalOptionsProvider,
    IOptions<IdGlobalSetupOptions_CUSTOMER> _globalCusotmerOptionsProvider,
    ILogger<TrustedDeviceRevokedConsumer> logger)
    : AEventHandler<TrustedDeviceRevokedIntegrationEvent>
{
    private readonly IdGlobalOptions _globalOptions = _globalOptionsProvider.Value;
    private readonly IdGlobalSetupOptions_CUSTOMER _globalCustomerOptions = _globalCusotmerOptionsProvider.Value;

    //---------------------------------------//

    public override async Task HandleEventAsync(TrustedDeviceRevokedIntegrationEvent data)
    {
        try
        {
            //Console.Beep();
            //logger.LogError("{message}", $"ForgotPasswordEvent: {data.Email}: {data.Phone}: {data.Name}: {data.IsCustomerTeam}");
            //Debug.WriteLine($"ForgotPasswordEvent: {data.Email}");


            //string accountsRoute = GetBaseUrl(data.IsCustomerTeam);
            //string pwdResetTknAddress = UrlBuilder.Combine(accountsRoute, IdGlobalConstants.EmailRoutes.ResetPassword);
            //string pwdResetTknUrl = $"{pwdResetTknAddress}?{IdGlobalConstants.EmailRoutes.Params.UserId}={data.UserId}&{IdGlobalConstants.EmailRoutes.Params.ResetToken}={data.ResetToken}";


            //var spec = new PasswordResetSpec(data.Name, data.Email, pwdResetTknUrl);
            //var eDetails = await emailDetailsTemplateGenerator.GenerateFromSpecAsync(spec);

            //var result = await emailService.SendEmailAsync(eDetails);

            //if (!result.Succeeded)
            //    logger.LogBasicResultFailure(result, IdErrorEvents.Email.ForgotPassword);

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
