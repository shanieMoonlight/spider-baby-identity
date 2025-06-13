using ClArch.ValueObjects;
using ID.Domain.Entities.Devices;
using ID.Domain.Utility.Exceptions;
using ID.Infrastructure.DomainServices.Teams.Dvcs;

namespace ID.Infrastructure.Tests.DmnSevcs.DvcsService;


public class AddDeviceTests : BaseDvcServiceTests
{

    //------------------------------------//

    [Fact]
    public async Task AddDeviceAsync_Success_ReturnsDeviceResult()
    {
        var teamId = Guid.NewGuid();
        var subId = Guid.NewGuid();
        var subPlanId = Guid.NewGuid();
        var dvcId = Guid.NewGuid();
        var teamWithSubs = TeamDataGenerator.CreateTeamWithSubsAndDvcs(teamId, subId, subPlanId, 0, dvcId);
        var team = teamWithSubs.Team;
        var sub = teamWithSubs.Subscription;
        var plan = teamWithSubs.SubscriptionPlan;
        var dvcs = sub.Devices;
        var dvcCount = dvcs.Count;
        var name = "DeviceName";
        var description = "DeviceDescription";
        var uniqueId = "qwertyuiop123456789";


        var service = new TeamDeviceService(_uowMock.Object, team, subId);

        // Act
        var result = await service.AddDeviceAsync(Name.Create(name), DescriptionNullable.Create(description), UniqueId.Create(uniqueId));

        // Assert
        result?.Value?.Name.ShouldBe(name);
        result?.Value?.Description.ShouldBe(description);
        result?.Value?.UniqueId.ShouldBe(uniqueId);
        team.Subscriptions.First(s => s.Id == subId).Devices.Count.ShouldBe(dvcCount + 1);
    }

    //------------------------------------//

    [Fact]
    public async Task AddDeviceAsync_ReturnsFailureResult_When_DeviceLimitExceededException()
    {
        var teamId = Guid.NewGuid();
        var subId = Guid.NewGuid();
        var subPlanId = Guid.NewGuid();
        var dvcId = Guid.NewGuid();
        var teamWithSubs = TeamDataGenerator.CreateTeamWithSubsAndDvcs(teamId, subId, subPlanId, 1, dvcId);
        var team = teamWithSubs.Team;
        var sub = teamWithSubs.Subscription;
        var plan = teamWithSubs.SubscriptionPlan;
        var dvcs = sub.Devices;
        var dvcCount = dvcs.Count;
        var name = "DeviceName";
        var description = "DeviceDescription";
        var uniqueId = "qwertyuiop123456789";

        var service = new TeamDeviceService(_uowMock.Object, team, subId);


        // Act
        var result = await service.AddDeviceAsync(Name.Create(name), DescriptionNullable.Create(description), UniqueId.Create(uniqueId));

        // Assert
        result?.Exception?.ShouldBeAssignableTo<DeviceLimitExceededException>();
        result?.Succeeded.ShouldBeFalse();
        result?.BadRequest.ShouldBeTrue();


    }

    //------------------------------------//

}//Cls

