using ClArch.ValueObjects;
using ID.Application.Features.Teams;
using ID.Application.Features.Teams.Cmd.Dvcs.AddDevice;
using ID.Domain.Abstractions.Services.Teams.Dvcs;
using ID.Domain.Entities.Devices;
using ID.Domain.Entities.Teams;
using ID.Tests.Data.Factories;
using Moq;
using MyResults;
using Shouldly;

namespace ID.Application.Tests.Teams.Dvcs;

public class AddDeviceToTeamHandlerTests
{
    private readonly Mock<ITeamDeviceServiceFactory> _deviceServiceFactoryMock;
    private readonly Mock<ITeamDeviceService> _deviceServiceMock;
    private readonly AddDeviceToTeamHandler _handler;

    //- - - - - - - - - - - - - - - - - - //

    public AddDeviceToTeamHandlerTests()
    {
        _deviceServiceFactoryMock = new Mock<ITeamDeviceServiceFactory>();
        _deviceServiceMock = new Mock<ITeamDeviceService>();
        _handler = new AddDeviceToTeamHandler(_deviceServiceFactoryMock.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnFailureResult_WhenGetServiceFails()
    {
        // Arrange
        var dto = new AddDeviceToTeamDto(Guid.NewGuid(), "Device1", "Test Device", "12345");
        var team = TeamDataFactory.Create();
        var request = new AddDeviceToTeamCmd(dto) { PrincipalTeam = team };
        var cancellationToken = CancellationToken.None;

        var getServiceResult = GenResult<ITeamDeviceService>.Failure("Service not found");

        _deviceServiceFactoryMock.Setup(x => x.GetServiceAsync(team, dto.SubscriptionId))
            .ReturnsAsync(getServiceResult);

        // Act
        var result = await _handler.Handle(request, cancellationToken);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldBe("Service not found");
        _deviceServiceFactoryMock.Verify(x => x.GetServiceAsync(team, dto.SubscriptionId), Times.Once);
        _deviceServiceMock.Verify(x => x.AddDeviceAsync(It.IsAny<Name>(), It.IsAny<DescriptionNullable>(), It.IsAny<UniqueId>()), Times.Never);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnSuccessResult_WhenDeviceIsAddedSuccessfully()
    {
        // Arrange
        var dto = new AddDeviceToTeamDto(Guid.NewGuid(), "Device1", "Test Device", "12345");
        var team = TeamDataFactory.Create();
        var request = new AddDeviceToTeamCmd(dto) { PrincipalTeam = team };
        var dvc1 = DeviceDataFactory.Create();
        var dvc2 = DeviceDataFactory.Create();
        var newDvc = DeviceDataFactory.Create();
        HashSet<TeamDevice> dvcs = [dvc1, dvc2];
        var subscription = SubscriptionDataFactory.Create(null, null, null, null, null, null, 5, dvcs);
        var subDto = subscription.ToDto();

        var mockService = new Mock<ITeamDeviceService>();
        mockService.Setup(s => s.AddDeviceAsync(It.IsAny<Name>(), It.IsAny<DescriptionNullable>(), It.IsAny<UniqueId>()))
                   .ReturnsAsync(GenResult<TeamDevice>.Success(newDvc));
        mockService.Setup(s => s.Subscription)
                  .Returns(subscription);

        _deviceServiceFactoryMock.Setup(f => f.GetServiceAsync(It.IsAny<Team>(), It.IsAny<Guid>()))
                                 .ReturnsAsync(GenResult<ITeamDeviceService>.Success(mockService.Object));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.ShouldBeAssignableTo<SubscriptionDto>();
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnFailureResult_WhenAddDeviceFails()
    {
        // Arrange
        var dto = new AddDeviceToTeamDto(Guid.NewGuid(), "Device1", "Test Device", "12345");
        var team = TeamDataFactory.Create();
        var dvc1 = DeviceDataFactory.Create();
        var dvc2 = DeviceDataFactory.Create();
        var newDvc = DeviceDataFactory.Create();
        HashSet<TeamDevice> dvcs = [dvc1, dvc2];
        var subscription = SubscriptionDataFactory.Create(null, null, null, null, null, null, 5, dvcs);
        var request = new AddDeviceToTeamCmd(dto) { PrincipalTeam = team };

        var mockService = new Mock<ITeamDeviceService>();
        mockService.Setup(s => s.AddDeviceAsync(It.IsAny<Name>(), It.IsAny<DescriptionNullable>(), It.IsAny<UniqueId>()))
                   .ReturnsAsync(GenResult<TeamDevice>.Failure("Add device failed"));

        mockService.Setup(s => s.Subscription)
                  .Returns(subscription);

        _deviceServiceFactoryMock.Setup(f => f.GetServiceAsync(It.IsAny<Team>(), It.IsAny<Guid>()))
                                 .ReturnsAsync(GenResult<ITeamDeviceService>.Success(mockService.Object));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldBe("Add device failed");
    }

    //------------------------------------//

}//Cls