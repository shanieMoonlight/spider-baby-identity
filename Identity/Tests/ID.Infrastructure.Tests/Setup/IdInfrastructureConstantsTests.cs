using ID.Infrastructure.Utility;

namespace ID.Infrastructure.Tests.Setup;
public class IdInfrastructureConstantsTests
{
    [Fact]
    public void AllPublicProperties_ShouldHaveRequiredModifier()
    {
        IdInfrastructureConstants.Jobs.DashboardPath.ShouldNotBeNullOrEmpty("DashboardPath should not be null or empty");
        IdInfrastructureConstants.Jobs.DashboardPath.ShouldStartWith("/");
        IdInfrastructureConstants.Jobs.DI_StorageKey.ShouldNotBeNullOrWhiteSpace();
        IdInfrastructureConstants.Jobs.Server.ShouldNotBeNullOrWhiteSpace();
        IdInfrastructureConstants.Jobs.Schema.ShouldNotBeNullOrWhiteSpace();
        IdInfrastructureConstants.Jobs.Queues.Default.ShouldNotBeNullOrWhiteSpace();
        IdInfrastructureConstants.Jobs.Queues.All.ShouldNotBeEmpty();

    }
}
