using ID.Email.Base.AppAbs;
using ID.Email.Base.Models;
using ID.Email.Base.Setup;
using ID.GlobalSettings.Setup.Options;
using Microsoft.Extensions.Options;

namespace ID.Email.Base.AppImps;

#pragma warning disable IDE1006 // Naming Styles
internal class EmailDetailsTemplateGenerator(
    IOptions<IdGlobalOptions> _globalOptionsProvider,
    ITemplateHelpers _templateHelpers,
    IOptions<IdEmailBaseOptions> _emailOptionsProvider)
    : IEmailDetailsTemplateGenerator
{
    private readonly IdGlobalOptions _globalOptions = _globalOptionsProvider.Value;
    private readonly IdEmailBaseOptions _emailOptions = _emailOptionsProvider.Value;

    private const string EMAIL_CONFIRMATION_MNTC_TEMPLATE_PATH = @"Assets\html-templates\EmailConfirmation\IdEmailConfirmationEmployee.html";
    private const string EMAIL_CONFIRMATION_CUSTOMER_TEMPLATE_PATH = @"Assets\html-templates\EmailConfirmation\IdEmailConfirmationCustomer.html";
    private const string PASSWORD_RESET_TEMPLATE_PATH = @"Assets\html-templates\ResetPassword\IdResetPassword.html";
    private const string TWO_FACTOR_TEMPLATE_PATH = @"Assets\html-templates\TwoFactor\IdTwoFactorLogin.html";
    private const string TWO_FACTOR_GOOGLE_AUTH_TEMPLATE_PATH = @"Assets\html-templates\IdTwoFactor\TwoFactorGoogleAuthSetup.html";
    private const string SUBSCRIPTION_PAUSED_TEMPLATE_PATH = @"Assets\html-templates\Subs\IdSubPaused.html";

    //------------------------------------//   

    public async Task<IEmailDetails> GenerateChangePasswordTemplateAsync(
       string toName,
       string toAddress,
       string callbackUrl) =>
     await _templateHelpers.GenerateTemplateWithCallback(
           toName,
           toAddress,
           callbackUrl,
           PASSWORD_RESET_TEMPLATE_PATH,
           $"New User - {_globalOptions.ApplicationName}"
       );

    //------------------------------------//

    public async Task<IEmailDetails> GenerateEmailConfirmationMntcTemplateAsync(
        string toName,
        string toAddress,
        string callbackUrl) =>
        await _templateHelpers.GenerateTemplateWithCallback(
            toName,
            toAddress,
            callbackUrl,
            EMAIL_CONFIRMATION_MNTC_TEMPLATE_PATH,
            $"New User - {_globalOptions.ApplicationName}"
        );

    //------------------------------------//

    public async Task<IEmailDetails> GenerateEmailConfirmationCustomerTemplateAsync(
        string toName,
        string toAddress,
        string callbackUrl) =>
      await _templateHelpers.GenerateTemplateWithCallback(
            toName,
            toAddress,
            callbackUrl,
            EMAIL_CONFIRMATION_CUSTOMER_TEMPLATE_PATH,
            $"New User - {_globalOptions.ApplicationName}"
        );

    //------------------------------------//

    public async Task<IEmailDetails> GeneratePasswordResetTemplateAsync(
        string toName,
        string toAddress,
        string callbackUrl) =>
      await _templateHelpers.GenerateTemplateWithCallback(
            toName,
            toAddress,
            callbackUrl,
            PASSWORD_RESET_TEMPLATE_PATH,
            $"Password Reset - {_globalOptions.ApplicationName}"
        );

    //------------------------------------//

    public async Task<IEmailDetails> GenerateTwoFactorTemplateAsync(
        string toName,
        string toAddress,
        string subject,
        string verificationCode)
    {
        var message = await _templateHelpers.ReadAndReplaceTemplateAsync(
            TWO_FACTOR_TEMPLATE_PATH,
            new Dictionary<string, string>
            {
                { EmailPlaceholders.PLACEHOLDER_USERNAME, toName },
                { EmailPlaceholders.PLACEHOLDER_VERIFICATION_CODE, verificationCode }
            });

        return new EmailDetails(
            EmailType.HTML,
            message,
            subject,
            toAddress,
            _emailOptions.BccAddresses,
            _emailOptions.FromAddress,
            _emailOptions.FromName
        );
    }

    //------------------------------------//

    public async Task<IEmailDetails> GenerateTwoFactorGoogleAuthTemplateAsync(
        string toName,
        string toAddress,
        string qrSrc,
        string manualQrCode,
        string subject = "Two-Factor Setup")
    {
        var message = await _templateHelpers.ReadAndReplaceTemplateAsync(
            TWO_FACTOR_GOOGLE_AUTH_TEMPLATE_PATH,
            new Dictionary<string, string>
            {                { EmailPlaceholders.PLACEHOLDER_USERNAME, toName },
                { EmailPlaceholders.PLACEHOLDER_2_FACTOR_PROVIDER, "Google Authenticator" },
                { EmailPlaceholders.PLACEHOLDER_MANUAL_QR_CODE, manualQrCode },
                { EmailPlaceholders.PLACEHOLDER_QR_IMG_SRC, qrSrc }
            });

        return new EmailDetails(
            EmailType.HTML,
            message,
            subject,
            toAddress,
            _emailOptions.BccAddresses,
            _emailOptions.FromAddress,
            _emailOptions.FromName
        );
    }

    //------------------------------------//

    public async Task<IEmailDetails> GenerateTwoFactorAuthTemplateAsync(
        string toName,
        string toAddress,
        string qrSrc,
        string manualQrCode,
        string provider,
        string subject = "Two-Factor Setup")
    {
        var message = await _templateHelpers.ReadAndReplaceTemplateAsync(
            TWO_FACTOR_GOOGLE_AUTH_TEMPLATE_PATH,
            new Dictionary<string, string>
            {                { EmailPlaceholders.PLACEHOLDER_USERNAME, toName },
                { EmailPlaceholders.PLACEHOLDER_2_FACTOR_PROVIDER, provider },
                { EmailPlaceholders.PLACEHOLDER_MANUAL_QR_CODE, manualQrCode },
                { EmailPlaceholders.PLACEHOLDER_QR_IMG_SRC, qrSrc },
            });

        return new EmailDetails(
            EmailType.HTML,
            message,
            subject,
            toAddress,
            _emailOptions.BccAddresses,
            _emailOptions.FromAddress,
            _emailOptions.FromName
        );
    }

    //------------------------------------//

    public async Task<IEmailDetails> GenerateSubscriptionPausedTemplateAsync(
        string toName,
        string toAddress,
        string subPlanName,
        string subject = "Subscription Paused")
    {
        var message = await _templateHelpers.ReadAndReplaceTemplateAsync(SUBSCRIPTION_PAUSED_TEMPLATE_PATH,
            new Dictionary<string, string>
            {
                { EmailPlaceholders.PLACEHOLDER_USERNAME, toName },
                { EmailPlaceholders.PLACEHOLDER_SUB_PLAN_NAME, subPlanName },
            });

        return new EmailDetails(
            EmailType.HTML,
            message,
            subject,
            toAddress,
            _emailOptions.BccAddresses,
            _emailOptions.FromAddress,
            _emailOptions.FromName
        );
    }

    //------------------------------------//
}

#pragma warning restore IDE1006 // Naming Styles