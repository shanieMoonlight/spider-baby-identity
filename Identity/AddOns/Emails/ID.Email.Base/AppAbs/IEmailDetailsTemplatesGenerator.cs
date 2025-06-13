namespace ID.Email.Base.AppAbs;

/// <summary>
/// Interface for sending some common emails
/// </summary>
public interface IEmailDetailsTemplateGenerator
{
    /// <summary>
    /// Generates IEmailDetails with template for getting an employee to confirm the email on their new account
    /// </summary>
    /// <param name="toName">Employee name</param>
    /// <param name="toAddress">Employee email</param>
    /// <param name="callbackUrl">The address of the EmailConfirmation page</param>
    /// <returns>IEmailDetails with template</returns>
    Task<IEmailDetails> GenerateEmailConfirmationMntcTemplateAsync(
        string toName,
        string toAddress,
        string callbackUrl);

    //- - - - - - - - - - - - - - - - - - - -//

    /// <summary>
    /// Generates IEmailDetails with template for change password email
    /// </summary>
    /// <param name="toName">Employee name</param>
    /// <param name="toAddress">Employee email</param>
    /// <param name="callbackUrl">The address of the EmailConfirmation page</param>
    /// <returns>IEmailDetails with template</returns>
    Task<IEmailDetails> GenerateChangePasswordTemplateAsync(
        string toName,
        string toAddress,
        string callbackUrl);

    //- - - - - - - - - - - - - - - - - - - -//

    /// <summary>
    /// Generates IEmailDetails with template for getting a customer to confirm the email on their new account
    /// </summary>
    /// <param name="toName">Employee name</param>
    /// <param name="toAddress">Employee email</param>
    /// <param name="callbackUrl">The address of the EmailConfirmation page</param>
    /// <returns>IEmailDetails with template</returns>
    Task<IEmailDetails> GenerateEmailConfirmationCustomerTemplateAsync(
        string toName,
        string toAddress,
        string callbackUrl);

    //- - - - - - - - - - - - - - - - - - - -//

    /// <summary>
    /// Generates IEmailDetails with template for resetting password email
    /// </summary>
    /// <param name="toName">Employee name</param>
    /// <param name="toAddress">Employee email</param>
    /// <param name="callbackUrl">The address of the EmailConfirmation page</param>
    /// <returns>IEmailDetails with template</returns>
    Task<IEmailDetails> GeneratePasswordResetTemplateAsync(
        string toName,
        string toAddress,
        string callbackUrl);

    //- - - - - - - - - - - - - - - - - - - -//

    /// <summary>
    /// Generates IEmailDetails with template for two-factor authentication email
    /// </summary>
    /// <param name="toName">Employee name</param>
    /// <param name="toAddress">Employee email</param>
    /// <param name="subject">Email subject</param>
    /// <param name="verificationCode">Login verification code</param>
    /// <returns>IEmailDetails with template</returns>
    Task<IEmailDetails> GenerateTwoFactorTemplateAsync(
        string toName,
        string toAddress,
        string subject,
        string verificationCode);

    //- - - - - - - - - - - - - - - - - - - -//

    /// <summary>
    /// Generates IEmailDetails with template for setting up Google Authenticator two-factor authentication
    /// </summary>
    /// <param name="toName">Employee name</param>
    /// <param name="toAddress">Employee email</param>
    /// <param name="qrSrc">QR code source for Google Authenticator</param>
    /// <param name="manualQrCode">Manual QR code for Google Authenticator</param>
    /// <param name="subject">Email subject</param>
    /// <returns>IEmailDetails with template</returns>
    Task<IEmailDetails> GenerateTwoFactorGoogleAuthTemplateAsync(
        string toName,
        string toAddress,
        string qrSrc,
        string manualQrCode,
        string subject = "Two-Factor Setup");

    //- - - - - - - - - - - - - - - - - - - -//

    /// <summary>
    /// Generates IEmailDetails with template for setting up two-factor authentication with a specified provider
    /// </summary>
    /// <param name="toName">Employee name</param>
    /// <param name="toAddress">Employee email</param>
    /// <param name="qrSrc">QR code source for the authentication provider</param>
    /// <param name="manualQrCode">Manual QR code for the authentication provider</param>
    /// <param name="provider">Name of the authentication provider</param>
    /// <param name="subject">Email subject</param>
    /// <returns>IEmailDetails with template</returns>
    Task<IEmailDetails> GenerateTwoFactorAuthTemplateAsync(
        string toName,
        string toAddress,
        string qrSrc,
        string manualQrCode,
        string provider,
        string subject = "Two-Factor Setup");

    //- - - - - - - - - - - - - - - - - - - -//

    /// <summary>
    /// Generates IEmailDetails with template for two-factor authentication email
    /// </summary>
    /// <param name="toName">Employee name</param>
    /// <param name="toAddress">Employee email</param>
    /// <param name="subject">Email subject</param>
    /// <returns>IEmailDetails with template</returns>
    Task<IEmailDetails> GenerateSubscriptionPausedTemplateAsync(
        string toName, 
        string toAddress, 
        string subPlanName, 
        string subject = "Subscription Paused");
}
