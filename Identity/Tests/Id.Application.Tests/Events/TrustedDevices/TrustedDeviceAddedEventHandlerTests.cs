namespace ID.Application.Tests.Events.TrustedDevices;

public class TrustedDeviceAddedEventHandlerTests
{
    [Fact]
    public async Task Handle_WhenDeviceFoundAndOwned_ShouldPublishDeviceAddedEvent()
    {
        // Arrange
        var finderMock = new Mock<ITrustedDeviceFinder>();
        var busMock = new Mock<ITrustedDeviceBus>();
        var loggerMock = new Mock<ILogger<TrustedDeviceAddedEventHandler>>();

        var handler = new TrustedDeviceAddedEventHandler(finderMock.Object, busMock.Object, loggerMock.Object);

        var team = TeamDataFactory.Create();
        var user = AppUserDataFactory.Create(team: team);
        var device = TrustedDeviceDataFactory.Create(user: user, dateCreated: DateTime.UtcNow);

        var domainEvent = new TrustedDeviceAddedDomainEvent(device.Id, user.Id);

        finderMock.Setup(f => f.FindWithUserAndTeamAsync(device.Id, user.Id)).ReturnsAsync(GenResult<TrustedDevice>.Success(device));

        // Act
        await handler.Handle(domainEvent, CancellationToken.None);

        // Assert
        busMock.Verify(b => b.PublishDeviceAddedEventAsync(
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
        var loggerMock = new Mock<ILogger<TrustedDeviceAddedEventHandler>>();

        var handler = new TrustedDeviceAddedEventHandler(finderMock.Object, busMock.Object, loggerMock.Object);

        var domainEvent = new TrustedDeviceAddedDomainEvent(Guid.NewGuid(), Guid.NewGuid());

        finderMock.Setup(f => f.FindWithUserAndTeamAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(GenResult<TrustedDevice>.NotFoundResult("not found"));

        // Act
        await handler.Handle(domainEvent, CancellationToken.None);

        // Assert
        busMock.Verify(b => b.PublishDeviceAddedEventAsync(It.IsAny<TrustedDevice>(), It.IsAny<AppUser>(), It.IsAny<Team>(), It.IsAny<CancellationToken>()), Times.Never);

        loggerMock.VerifyErrorLogging(IdErrorEvents.Listeners.TrustedDeviceAdded, Times.AtLeastOnce);
    }

    //--------------------//

    [Fact]
    public async Task Handle_WhenFinderReturnsForbidden_ShouldLogErrorAndNotPublish()
    {
        // Arrange
        var finderMock = new Mock<ITrustedDeviceFinder>();
        var busMock = new Mock<ITrustedDeviceBus>();
        var loggerMock = new Mock<ILogger<TrustedDeviceAddedEventHandler>>();

        var handler = new TrustedDeviceAddedEventHandler(finderMock.Object, busMock.Object, loggerMock.Object);

        var team = TeamDataFactory.Create();
        var owner = AppUserDataFactory.Create(team: team);
        var otherUser = AppUserDataFactory.Create();
        var device = TrustedDeviceDataFactory.Create(user: owner, dateCreated: DateTime.UtcNow);

        var domainEvent = new TrustedDeviceAddedDomainEvent(device.Id, otherUser.Id);

        // Finder should indicate forbidden (user is not owner)
        finderMock.Setup(f => f.FindWithUserAndTeamAsync(device.Id, otherUser.Id)).ReturnsAsync(GenResult<TrustedDevice>.ForbiddenResult("not owner"));

        // Act
        await handler.Handle(domainEvent, CancellationToken.None);

        // Assert
        busMock.Verify(b => b.PublishDeviceAddedEventAsync(It.IsAny<TrustedDevice>(), It.IsAny<AppUser>(), It.IsAny<Team>(), It.IsAny<CancellationToken>()), Times.Never);
        loggerMock.VerifyErrorLogging(IdErrorEvents.Listeners.TrustedDeviceAdded, Times.AtLeastOnce);
    }
}
