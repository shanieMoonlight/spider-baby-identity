using ID.GlobalSettings.Constants;
using ID.GlobalSettings.Setup.Options;
using ID.GlobalSettings.Utility;

namespace ID.Email.Base.EventListeners.TrustedDevices;
internal class TrustedDeviceConsumerUtils
{

    internal static string GetChangePwdUrl(
        bool isCustomerTeam,
        IdGlobalOptions globalOptions,
        IdGlobalSetupOptions_CUSTOMER globalCustomerOptions)
    {
        string accountsRoute = GetBaseUrl(isCustomerTeam, globalOptions, globalCustomerOptions);
        return UrlBuilder.Combine(accountsRoute, IdGlobalConstants.EmailRoutes.ChangePassword);
    }

    //---------------------//

    internal static string GetTrustedDeviceMgmtUrl(
        bool isCustomerTeam,
        IdGlobalOptions globalOptions,
        IdGlobalSetupOptions_CUSTOMER globalCustomerOptions)
    {
        string accountsRoute = GetBaseUrl(isCustomerTeam, globalOptions, globalCustomerOptions);
        return UrlBuilder.Combine(accountsRoute, IdGlobalConstants.EmailRoutes.TrustedDevices);
    }

    //---------------------//

    private static string GetBaseUrl(
        bool isCustomerTeam,
        IdGlobalOptions globalOptions,
        IdGlobalSetupOptions_CUSTOMER globalCustomerOptions) =>
        isCustomerTeam
            ? globalCustomerOptions.CustomerAccountsUrl
            : globalOptions.MntcAccountsUrl;



}//Cls
