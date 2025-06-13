using ID.Domain.Entities.Teams;
using ID.Domain.Utility.Messages;
using ID.Infrastructure.DomainServices.Teams.Dvcs;
using ID.Infrastructure.Persistance.Abstractions.Repos;
using ID.Tests.Data.Factories;
using Moq;
using Shouldly;

namespace ID.Infrastructure.Tests.DmnSevcs.DvcsService;
public class RemoveDeviceTests : BaseDvcServiceTests
{
    //------------------------------------//

    [Fact]
    public async Task RemoveDeviceAsync_NotFound_ReturnsNotFoundResult()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var subId = Guid.NewGuid();
        var subPlanId = Guid.NewGuid();
        var dvcId = Guid.NewGuid();
        var teamWithSubs = TeamDataGenerator.CreateTeamWithSubsAndDvcs(teamId, subId, subPlanId, 0, dvcId);
        var team = teamWithSubs.Team;
        var subscription = teamWithSubs.Subscription;
        var teamDeviceService = new TeamDeviceService(_uowMock.Object, team, subscription.Id);

        var alreadyRemovedDeviceId = Guid.NewGuid();
        var alreadyRemovedDevice = DeviceDataFactory.Create(alreadyRemovedDeviceId);

        // Act
        var result = await teamDeviceService.RemoveDeviceAsync(alreadyRemovedDeviceId);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.NotFound.ShouldBeTrue();
        result.Info.ShouldBe(IDMsgs.Error.NotFound<TeamDevice>(alreadyRemovedDeviceId));
    }

    //------------------------------------//

    [Fact]
    public async Task RemoveDeviceAsync_False_ReturnsNotFoundResult()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var subId = Guid.NewGuid();
        var subPlanId = Guid.NewGuid();
        var dvcId = Guid.NewGuid();
        var teamWithSubs = TeamDataGenerator.CreateTeamWithSubsAndDvcs(teamId, subId, subPlanId, 0, dvcId);
        var team = teamWithSubs.Team;
        var subscription = teamWithSubs.Subscription;
        var teamDeviceService = new TeamDeviceService(_uowMock.Object, team, subscription.Id);

        var alreadyRemovedDeviceId = Guid.NewGuid();
        var alreadyRemovedDevice = DeviceDataFactory.Create(alreadyRemovedDeviceId);

        // Act
        var result = await teamDeviceService.RemoveDeviceAsync(alreadyRemovedDevice);

        // Assert
        result.ShouldBeFalse();
    }

    //------------------------------------//

    [Fact]
    public async Task RemoveDeviceAsync_RemovesDeviceAndSaves_WhenDeviceExists()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var subId = Guid.NewGuid();
        var subPlanId = Guid.NewGuid();
        var dvcId = Guid.NewGuid();
        var teamWithSubs = TeamDataGenerator.CreateTeamWithSubsAndDvcs(teamId, subId, subPlanId, 0, dvcId);
        var team = teamWithSubs.Team;
        var subscription = teamWithSubs.Subscription;
        var teamDevice = teamWithSubs.Device;
        var teamDeviceService = new TeamDeviceService(_uowMock.Object, team, subscription.Id);

        // Act
        var result = await teamDeviceService.RemoveDeviceAsync(teamDevice.Id);

        // Assert
        result.Succeeded.ShouldBeTrue();
        subscription.Devices.ShouldNotContain(teamDevice);
        _uowMock.Verify(u => u.TeamRepo.UpdateAsync(team), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }


    //------------------------------------//

    [Fact]
    public async Task RemoveDeviceAsync_RemovesDeviceAndSaves_WhenDeviceExists_2()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var subId = Guid.NewGuid();
        var subPlanId = Guid.NewGuid();
        var dvcId = Guid.NewGuid();
        var teamWithSubs = TeamDataGenerator.CreateTeamWithSubsAndDvcs(teamId, subId, subPlanId, 0, dvcId);
        var team = teamWithSubs.Team;
        var subscription = teamWithSubs.Subscription;
        var teamDevice = teamWithSubs.Device;
        var teamDeviceService = new TeamDeviceService(_uowMock.Object, team, subscription.Id);

        // Act
        var result = await teamDeviceService.RemoveDeviceAsync(teamDevice);

        // Assert
        result.ShouldBeTrue();
        subscription.Devices.ShouldNotContain(teamDevice);
        _uowMock.Verify(u => u.TeamRepo.UpdateAsync(team), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }


    //------------------------------------//

    [Fact]
    public async Task RemoveDeviceAsync_ReturnsNotFoundResult_WhenDeviceIsNotRemoved()
    {
        // Arrange
        var _uowMock = new Mock<IIdUnitOfWork>();
        var teamId = Guid.NewGuid();
        var subId = Guid.NewGuid();
        var subPlanId = Guid.NewGuid();
        var dvcId = Guid.NewGuid();
        var teamWithSubs = TeamDataGenerator.CreateTeamWithSubsAndDvcs(teamId, subId, subPlanId, 0, dvcId);
        var team = teamWithSubs.Team;
        var subscription = teamWithSubs.Subscription;
        var teamDevice = teamWithSubs.Device;
        var teamDeviceService = new TeamDeviceService(_uowMock.Object, team, subscription.Id);

        var alreadyRemovedDeviceId = Guid.NewGuid();
        var alreadyRemovedDevice = DeviceDataFactory.Create(alreadyRemovedDeviceId);

        // Act
        var result = await teamDeviceService.RemoveDeviceAsync(alreadyRemovedDeviceId);

        // Assert
        result.Succeeded.ShouldBeFalse();
        subscription.Devices.ShouldContain(teamDevice);
        _uowMock.Verify(u => u.TeamRepo.UpdateAsync(team), Times.Never);
        _uowMock.Verify(u => u.SaveChangesAsync(default), Times.Never);
    }

    //------------------------------------//

    [Fact]
    public async Task RemoveDeviceAsync_DoesNotSave_WhenDeviceIsNotRemoved()
    {
        // Arrange
        var _uowMock = new Mock<IIdUnitOfWork>();
        var teamId = Guid.NewGuid();
        var subId = Guid.NewGuid();
        var subPlanId = Guid.NewGuid();
        var dvcId = Guid.NewGuid();
        var teamWithSubs = TeamDataGenerator.CreateTeamWithSubsAndDvcs(teamId, subId, subPlanId, 0, dvcId);
        var team = teamWithSubs.Team;
        var subscription = teamWithSubs.Subscription;
        var teamDevice = teamWithSubs.Device;
        var teamDeviceService = new TeamDeviceService(_uowMock.Object, team, subscription.Id);

        var alreadyRemovedDeviceId = Guid.NewGuid();
        var alreadyRemovedDevice = DeviceDataFactory.Create(alreadyRemovedDeviceId);

        // Act
        var result = await teamDeviceService.RemoveDeviceAsync(alreadyRemovedDevice);

        // Assert
        result.ShouldBeFalse();
        subscription.Devices.ShouldContain(teamDevice);
        _uowMock.Verify(u => u.TeamRepo.UpdateAsync(team), Times.Never);
        _uowMock.Verify(u => u.SaveChangesAsync(default), Times.Never);
    }

    //------------------------------------//

}//Cls
