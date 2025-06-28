using ID.API.Setup;
using ID.API.Setup.ExtraSetup;
using ID.Application.Customers.Setup;
using ID.Demo.TestControllers.Setup;
using ID.Email.SG.Setup;
using ID.Email.SMTP.Setup;
using ID.Infrastructure.Auth.JWT.Setup;
using ID.Msg.Twilio.Setup;
using ID.OAuth.Google.Setup;
using ID.PhoneConfirmation.Setup;
using MyIdDemo.Setup.Data;
using MyIdDemo.Setup.Utils;

namespace MyIdDemo.Setup;



/// <summary>
/// Install Identity stuff
/// </summary>
public class MyIdInstaller : IServiceInstaller
{
    /// <summary>
    /// Install some Identity dependencies
    /// </summary>
    /// <param name="builder">Application Builder</param>
    /// <param name="startupData">All the app config and settings</param>
    public WebApplicationBuilder Install(WebApplicationBuilder builder, StartupData startupData)
    {
        var isDev = builder.Environment.IsDevelopment();

        builder.Services
            .AddMyIdDemoTestControllers();

        //builder.Services.AddMyId<TeamRole_User_to_Mgr_ClaimsGenerator>( // Uncomment this to use the TeamRoles User to Admin feature
        builder.Services.AddMyId(
           config =>
           {
               config.ApplicationName = startupData.APP_NAME;
               config.ConnectionString = startupData.ConnectionStringsSection.GetSqlDb();
               //config.TokenSigningKey = startupData.IdentitySection.GetSymetricKey();
               config.JwtAsymmetricPrivateKey_Xml = startupData.GetAsymmetricPrivateKeyXmlString();
               config.JwtAsymmetricPublicKey_Xml = startupData.GetAsymmetricPublicKeyXmlString();
               config.JwtTokenExpirationMinutes = isDev ? 1000000 : startupData.IdentitySection.GetJwtExpirationMinutes();
               config.JwtRefreshTokensEnabled = true;
               config.JwtRefreshTokenUpdatePolicy = RefreshTokenUpdatePolicy.HalfLife;
               config.MntcAccountsUrl = startupData.IdentitySection.GetMntcRoute();
               config.AllowExternalPagesDevModeAccess = false;
               config.FromMoblieAppHeaderValue = "MobileApp-TestValue";
               config.UseSeperateEventBus = true;
               config.PasswordOptions = new ID.Infrastructure.Setup.Passwords.IdPasswordOptions()
               {
                   RequireDigit = true,
                   RequireLowercase = true,
                   RequireNonAlphanumeric = true,
                   RequireUppercase = true,
                   RequiredLength = 6,
                   RequiredUniqueChars = 1
               };
           })
           .Services
           .AddMyIdMessagingTwilio(config =>
           {
               config.TwilioFromNumber = startupData.TwilioOptionsSection.GetFrom();
               config.TwilioPassword = startupData.TwilioOptionsSection.GetPassword();
               config.TwilioId = startupData.TwilioOptionsSection.GetId();
           })
           .AddMyIdCustomers(config =>
           {
               config.CustomerAccountsUrl = startupData.IdentitySection.GetCustomerRoute();
           })
           .AddMyIdPhoneConfirmation()
           .AddMyIdOAuthGoogle(config =>
           {
               config.ClientId = startupData.OAuthSection.GoogleSection.GetClientId()!;
               config.ClientSecret = startupData.OAuthSection.GoogleSection.GetClientSecret()!;
           })
           //.AddTeamRolesUserToAdmin()// Uncomment this to add the TeamRoles User to Admin Policies
           ;




        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddMyIdEmailSmtp(config =>
            {
                config.SmtpServerAddress = startupData.SmtpSettingsSection.GetServerAddress();
                config.SmtpPortNumber = startupData.SmtpSettingsSection.GetPortNumber();
                config.SmtpUsernameOrEmail = startupData.SmtpSettingsSection.GetUsernameOrEmail();
                config.SmtpPassword = startupData.SmtpSettingsSection.GetPassword();

                config.BccAddresses = startupData.IdentitySection.GetBccAddresses()!;
                config.LogoUrl = startupData.GetLogoUrl();
                config.FromAddress = startupData.IdentitySection.GetFromAddress()!;
                config.ColorHexBrand = startupData.COLOR_HEX_BRAND;
                config.FromName = startupData.IdentitySection.GetFromName();
            });
        }
        else
        {

            builder.Services.AddMyIdEmailSG(config =>
            {
                config.ApiKey = startupData.SendGridSettingsSection.GetApiKey()!;

                config.BccAddresses = startupData.IdentitySection.GetBccAddresses()!;
                config.FromAddress = startupData.IdentitySection.GetFromAddress()!;
                config.LogoUrl = startupData.GetLogoUrl();
                config.ColorHexBrand = startupData.COLOR_HEX_BRAND;
                config.FromName = startupData.COMPANY_NAME;
            });
        }

        return builder;
    }

}//Cls


//######################################//


/// <summary>
/// Install Identity stuff
/// </summary>
public static class MyIdInstallerExtensions
{

    /// <summary>
    /// Install some Identity dependencies
    /// </summary>
    /// <param name="builder">Application Builder</param>
    /// <param name="startupData">All the app config and settings</param>
    public static WebApplicationBuilder InstallMyId(this WebApplicationBuilder builder, StartupData startupData) =>
        new MyIdInstaller().Install(builder, startupData);

}//Cls



//######################################//