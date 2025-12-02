using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Shouldly;
using ID.Application.AppImps.EventBuses;
using ID.Application.AppAbs.EventBuses;
using ID.IntegrationEvents.Abstractions;
using ID.IntegrationEvents.Events.Account.TrustedDevices;
using ID.Domain.Entities.TrustedDevices;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using Xunit;

namespace ID.Application.Tests.AppImps.EventBuses;

public class TrustedDeviceBusTests
{
    [Fact]
    public async Task PublishDeviceAddedEventAsync_CallsEventBusWithAddedEvent()
    {
        // Arrange
        var busMock = new Mock<IEventBus>();
        var trustedDeviceBus = new TrustedDeviceBus(busMock.Object);

        var device = TrustedDeviceDataFactory.Create();
        var user = AppUserDataFactory.Create();
        var team = TeamDataFactory.Create();

        // Act
        await trustedDeviceBus.PublishDeviceAddedEventAsync(device, user, team, CancellationToken.None);

        // Assert
        busMock.Verify(b => b.PublishAsync(It.Is<TrustedDeviceAddedIntegrationEvent>(e =>
            e.DeviceId == device.Id &&
            e.UserEmail == (user.Email ?? string.Empty) &&
            e.UserName == (user.FirstName ?? user.UserName ?? "User")
        ), It.IsAny<CancellationToken>()), Times.Once);
    }

    //--------------------//

    [Fact]
    public async Task PublishDeviceRevokedEventAsync_CallsEventBusWithRevokedEvent()
    {
        // Arrange
        var busMock = new Mock<IEventBus>();
        var trustedDeviceBus = new TrustedDeviceBus(busMock.Object);

        var device = TrustedDeviceDataFactory.Create();
        var user = AppUserDataFactory.Create();
        var team = TeamDataFactory.Create();

        // Act
        await trustedDeviceBus.PublishDeviceRevokedEventAsync(device, user, team, CancellationToken.None);

        // Assert
        busMock.Verify(b => b.PublishAsync(It.Is<TrustedDeviceRevokedIntegrationEvent>(e =>
            e.DeviceId == device.Id &&
            e.UserEmail == (user.Email ?? string.Empty) &&
            e.UserName == (user.FirstName ?? user.UserName ?? "User")
        ), It.IsAny<CancellationToken>()), Times.Once);
    }
}
