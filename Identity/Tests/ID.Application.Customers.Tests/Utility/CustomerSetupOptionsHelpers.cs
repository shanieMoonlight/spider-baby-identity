using ID.GlobalSettings.Setup.Defaults;
using ID.GlobalSettings.Setup.Options;

namespace ID.Infrastructure.Tests.Utility;

internal class CustomerSetupOptionsHelpers
{
    /// <summary>
    /// Creates a valid default instance of <see cref="IdDomain_Customer_SetupOptions"/>.
    /// </summary>
    /// <returns>A valid <see cref="IdDomain_Customer_SetupOptions"/> instance.</returns>
    public static IdGlobalSetupOptions_CUSTOMER CreateValidDefaultCustomerSetupOptions()
    {
        return new IdGlobalSetupOptions_CUSTOMER
        {
            MaxTeamPosition = IdGlobalDefaultValues.Customer.MAX_TEAM_POSITION,
            MinTeamPosition = IdGlobalDefaultValues.Customer.MIN_TEAM_POSITION,
            MaxTeamSize = IdGlobalDefaultValues.Customer.MAX_TEAM_SIZE
        };
    }
}
