using ID.GlobalSettings.Setup.Options;

namespace ID.Tests.Data.GlobalOptions;
public class GlobalOptionsUtils
{
    public static IdGlobalOptions ValidOptions => new()
    {
        ApplicationName = "Test Application",
        MntcAccountsUrl = "https://mntc.example.com/accounts",
        MntcTeamMaxPosition = 10,
        MntcTeamMinPosition = 1,
        SuperTeamMinPosition = 1,
        SuperTeamMaxPosition = 10,
        ClaimTypePrefix = "test_claim",
        JwtRefreshTokensEnabled = true,
        PhoneTokenTimeSpan = TimeSpan.FromMinutes(15)
    };

    //------------------------------//
    public static IdGlobalOptions InitiallyValidOptions(
        string? applicationName = null,
        string? mntcAccountsUrl = null,
        int? defaultMaxTeamPosition = null,
        int? defaultMinTeamPosition = null,
        int? superTeamMinPosition = null,
        int? superTeamMaxPosition = null,
        string? claimTypePrefix = null,
        bool? refreshTokensEnabled = null,
        TimeSpan? phoneTokenTimeSpan = null)
    {
        return new()
        {
            ApplicationName = applicationName ?? "Test Application",
            MntcAccountsUrl = mntcAccountsUrl ?? "https://mntc.example.com/accounts",
            MntcTeamMaxPosition = defaultMaxTeamPosition ?? 10,
            MntcTeamMinPosition = defaultMinTeamPosition ?? 1,
            SuperTeamMinPosition = superTeamMinPosition ?? 1,
            SuperTeamMaxPosition = superTeamMaxPosition ?? 10,
            ClaimTypePrefix = claimTypePrefix ?? "test_claim",
            JwtRefreshTokensEnabled = refreshTokensEnabled ?? true,
            PhoneTokenTimeSpan = phoneTokenTimeSpan ?? TimeSpan.FromMinutes(15)
        };
    }

    //------------------------------//

    public static IdGlobalSetupOptions_CUSTOMER InitiallyValidCustomerOptions(
        string? customerAccountsUrl = null,
        int? maxTeamPosition = null,
        int? minTeamPosition = null,
        int? maxTeamSize = null)
    {
        return new()
        {
            CustomerAccountsUrl = customerAccountsUrl ?? "https://customer.example.com/accounts",
            MaxTeamPosition = maxTeamPosition ?? 5,
            MinTeamPosition = minTeamPosition ?? 1,
            MaxTeamSize = maxTeamSize ?? 20
        };
    }

    //------------------------------//


}
