using ID.Infrastructure.DomainServices.Teams.Dvcs;
using Shouldly;

namespace ID.Infrastructure.Tests.DmnSevcs.DvcsService;


public class GetDeviceTests : BaseDvcServiceTests
{

    //------------------------------------//

    [Fact]
    public async Task GetDeviceAsync_ExistingSubscription_ReturnsDevice()
    {
        var teamId = Guid.NewGuid();
        var subId = Guid.NewGuid();
        var subPlanId = Guid.NewGuid();
        var dvcId = Guid.NewGuid();
        var teamWithSubs = TeamDataGenerator.CreateTeamWithSubsAndDvcs(teamId, subId, subPlanId, 0, dvcId);
        var team = teamWithSubs.Team;

        var service = new TeamDeviceService(_uowMock.Object, team, subId);

        var result = await service.GetDeviceAsync(dvcId);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(dvcId);
    }

    //------------------------------------//

    [Fact]
    public async Task GetDeviceAsync_NonExistentDevice_ReturnsNull()
    {
        var teamId = Guid.NewGuid();
        var subId = Guid.NewGuid();
        var subPlanId = Guid.NewGuid();
        var dvcId = Guid.NewGuid();
        var teamWithSubs = TeamDataGenerator.CreateTeamWithSubsAndDvcs(teamId, subId, subPlanId, 0, dvcId);
        var team = teamWithSubs.Team;

        var service = new TeamDeviceService(_uowMock.Object, team, subId);

        var result = await service.GetDeviceAsync(Guid.NewGuid());

        result.ShouldBeNull();
    }

    //------------------------------------//

}//Cls
