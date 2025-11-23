namespace ID.Application.Tests.Events.TrustedDevices;

public class TrustedDeviceRevokedEventHandlerTests
{
    [Fact]
    public async Task Handle_WhenDeviceFoundAndOwned_ShouldPublishDeviceRevokedEvent()
    {
        // Arrange
        var finderMock = new Mock<ITrustedDeviceFinder>();
        var busMock = new Mock<ITrustedDeviceBus>();
        var loggerMock = new Mock<ILogger<TrustedDeviceRevokedEventHandler>>();

        var handler = new TrustedDeviceRevokedEventHandler(finderMock.Object, busMock.Object, loggerMock.Object);

        var team = TeamDataFactory.Create();
        var user = AppUserDataFactory.Create(team: team);
        var device = TrustedDeviceDataFactory.Create(user: user, dateCreated: DateTime.UtcNow);

        var domainEvent = new TrustedDeviceRevokedDomainEvent(device.Id, user.Id);

        finderMock.Setup(f => f.FindWithUserAndTeamAsync(device.Id, user.Id)).ReturnsAsync(GenResult<TrustedDevice>.Success(device));

        // Act
        await handler.Handle(domainEvent, CancellationToken.None);

        // Assert
        busMock.Verify(b => b.PublishDeviceRevokedEventAsync(
            It.Is<TrustedDevice>(d => d == device),
            It.Is<AppUser>(u => u == user),
            It.Is<Team>(t => t == team),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    //--------------------//

    [Fact]
    public async Task Handle_WhenDeviceNotFound_ShouldLogErrorAndNotPublish()
    {
        // Arrange
        var finderMock = new Mock<ITrustedDeviceFinder>();
        var busMock = new Mock<ITrustedDeviceBus>();
        var loggerMock = new Mock<ILogger<TrustedDeviceRevokedEventHandler>>();

        var handler = new TrustedDeviceRevokedEventHandler(finderMock.Object, busMock.Object, loggerMock.Object);

        var domainEvent = new TrustedDeviceRevokedDomainEvent(Guid.NewGuid(), Guid.NewGuid());

        finderMock.Setup(f => f.FindWithUserAndTeamAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(GenResult<TrustedDevice>.NotFoundResult("not found"));

        // Act
        await handler.Handle(domainEvent, CancellationToken.None);

        // Assert
        busMock.Verify(b => b.PublishDeviceRevokedEventAsync(It.IsAny<TrustedDevice>(), It.IsAny<AppUser>(), It.IsAny<Team>(), It.IsAny<CancellationToken>()), Times.Never);
        loggerMock.VerifyErrorLogging(IdErrorEvents.Listeners.TrustedDeviceRevoked, Times.AtLeastOnce);
    }

    //--------------------//

    [Fact]
    public async Task Handle_WhenFinderReturnsForbidden_ShouldLogErrorAndNotPublish()
    {
        // Arrange
        var finderMock = new Mock<ITrustedDeviceFinder>();
        var busMock = new Mock<ITrustedDeviceBus>();
        var loggerMock = new Mock<ILogger<TrustedDeviceRevokedEventHandler>>();

        var handler = new TrustedDeviceRevokedEventHandler(finderMock.Object, busMock.Object, loggerMock.Object);

        var owner = AppUserDataFactory.Create();
        var otherUser = AppUserDataFactory.Create();
        var device = TrustedDeviceDataFactory.Create(user: owner, dateCreated: DateTime.UtcNow);

        var domainEvent = new TrustedDeviceRevokedDomainEvent(device.Id, otherUser.Id);

        // Finder should indicate forbidden (user is not owner)
        finderMock.Setup(f => f.FindWithUserAndTeamAsync(device.Id, otherUser.Id)).ReturnsAsync(GenResult<TrustedDevice>.ForbiddenResult("not owner"));

        // Act
        await handler.Handle(domainEvent, CancellationToken.None);

        // Assert
        busMock.Verify(b => b.PublishDeviceRevokedEventAsync(It.IsAny<TrustedDevice>(), It.IsAny<AppUser>(), It.IsAny<Team>(), It.IsAny<CancellationToken>()), Times.Never);
        loggerMock.VerifyErrorLogging(IdErrorEvents.Listeners.TrustedDeviceRevoked, Times.AtLeastOnce);
    }
}
