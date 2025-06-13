using ID.Application.Features.Teams.Cmd.Dvcs.RemoveDevice;
using ID.Domain.Abstractions.Services.Teams.Dvcs;
using ID.Tests.Data.Factories;
using Moq;
using MyResults;
using Shouldly;

namespace ID.Application.Tests.Features.Teams.Cmd.@Dvcs.Delete;
public class RemoveDeviceFromTeamSubscriptionHandlerTests
{
    private readonly RemoveDeviceFromTeamSubscriptionHandler _handler;
    private readonly Mock<ITeamDeviceServiceFactory> _deviceServiceFactoryMock;
    private readonly Mock<ITeamDeviceService> _deviceServiceMock;

    //- - - - - - - - - - - - - - - - - - //

    public RemoveDeviceFromTeamSubscriptionHandlerTests()
    {
        _deviceServiceFactoryMock = new Mock<ITeamDeviceServiceFactory>();
        _deviceServiceMock = new Mock<ITeamDeviceService>();
        _handler = new RemoveDeviceFromTeamSubscriptionHandler(_deviceServiceFactoryMock.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_SuccessfullyRemovesDeviceFromSubscription()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();
        var dvcCount = 2;
        var dvcs = DeviceDataFactory.CreateMany(dvcCount).ToHashSet();
        var dvcToRemove = dvcs.First();
        var subscription = SubscriptionDataFactory.Create(subscriptionId, null, null, null, null, null, 0, dvcs);
        var dto = new RemoveDeviceFromTeamSubscriptionDto(subscription.Id, default);
        var team = TeamDataFactory.Create(teamId, null, null, [subscription]);

        _deviceServiceMock.Setup(s => s.RemoveDeviceAsync(dto.DeviceId))
                 .ReturnsAsync(GenResult<bool>.Success(true));

        _deviceServiceMock.Setup(s => s.Subscription)
                  .Returns(subscription);

        _deviceServiceFactoryMock.Setup(f => f.GetServiceAsync(team, dto.SubscriptionId))
            .ReturnsAsync(GenResult<ITeamDeviceService>.Success(_deviceServiceMock.Object));

        var cmd = new RemoveDeviceFromTeamSubscriptionCmd(dto)
        {
            PrincipalTeam = team
        };

        // Act
        var result = await _handler.Handle(cmd, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Id.ShouldBe(subscriptionId);
        _deviceServiceMock.Verify(m => m.RemoveDeviceAsync(It.IsAny<Guid>()), Times.Once);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnFailureResult_WhenServiceNotFound()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();
        var dvcCount = 2;
        var dvcs = DeviceDataFactory.CreateMany(dvcCount).ToHashSet();
        var dvcToRemove = dvcs.First();
        var subscription = SubscriptionDataFactory.Create(subscriptionId, null, null, null, null, null, 0, dvcs);
        var dto = new RemoveDeviceFromTeamSubscriptionDto(subscription.Id, default);
        var team = TeamDataFactory.Create(teamId, null, null, [subscription]);

        var request = new RemoveDeviceFromTeamSubscriptionCmd(dto) { PrincipalTeam = team };

        _deviceServiceFactoryMock.Setup(f => f.GetServiceAsync(team, dto.SubscriptionId))
                 .ReturnsAsync(GenResult<ITeamDeviceService>.NotFoundResult("Service not found"));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.NotFound.ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnFailureResult_WhenRemoveDeviceFails()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();
        var dvcCount = 2;
        var dvcs = DeviceDataFactory.CreateMany(dvcCount).ToHashSet();
        var dvcToRemove = dvcs.First();
        var subscription = SubscriptionDataFactory.Create(subscriptionId, null, null, null, null, null, 0, dvcs);
        var dto = new RemoveDeviceFromTeamSubscriptionDto(subscription.Id, default);
        var team = TeamDataFactory.Create(teamId, null, null, [subscription]);

        _deviceServiceMock.Setup(s => s.RemoveDeviceAsync(dto.DeviceId))
                .ReturnsAsync(GenResult<bool>.Failure("Failed to remove device"));

        _deviceServiceMock.Setup(s => s.Subscription)
                  .Returns(subscription);

        _deviceServiceFactoryMock.Setup(f => f.GetServiceAsync(team, dto.SubscriptionId))
            .ReturnsAsync(GenResult<ITeamDeviceService>.Success(_deviceServiceMock.Object));

        var cmd = new RemoveDeviceFromTeamSubscriptionCmd(dto)
        {
            PrincipalTeam = team
        };

        // Act
        var result = await _handler.Handle(cmd, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldBe("Failed to remove device");
    }

    //------------------------------------//


}//Cls
