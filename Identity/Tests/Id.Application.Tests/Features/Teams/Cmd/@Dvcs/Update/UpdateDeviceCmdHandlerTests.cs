using ClArch.ValueObjects;
using ID.Application.Features.Teams.Cmd.Dvcs;
using ID.Application.Features.Teams.Cmd.Dvcs.UpdateDevice;
using ID.Domain.Abstractions.Services.Teams.Dvcs;
using ID.Domain.Entities.Teams;
using ID.Tests.Data.Factories;
using Moq;
using MyResults;
using Shouldly;

namespace ID.Application.Tests.Teams.Dvcs;
public class UpdateDeviceCmdHandlerTests
{
    private readonly UpdateDeviceHandler _handler;
    private readonly Mock<ITeamDeviceServiceFactory> _deviceServiceFactoryMock;
    private readonly Mock<ITeamDeviceService> _deviceServiceMock;

    //- - - - - - - - - - - - - - - - - - //

    public UpdateDeviceCmdHandlerTests()
    {
        _deviceServiceFactoryMock = new Mock<ITeamDeviceServiceFactory>();
        _deviceServiceMock = new Mock<ITeamDeviceService>();
        _handler = new UpdateDeviceHandler(_deviceServiceFactoryMock.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnSuccessResult_WhenDeviceIsUpdatedSuccessfully()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var device = DeviceDataFactory.Create();
        var dto = new DeviceDto
        {
            Id = Guid.NewGuid(),
            Name = "DeviceName",
            Description = "DeviceDescription",
            SubscriptionId = Guid.NewGuid()
        };
        var subscriptionId = Guid.NewGuid();
        var dvcCount = 2;
        var dvcs = DeviceDataFactory.CreateMany(dvcCount).ToHashSet();
        var dvcToRemove = dvcs.First();
        var subscription = SubscriptionDataFactory.Create(subscriptionId, null, null, null, null, null, 0, dvcs);
        var request = new UpdateDeviceCmd(dto) { PrincipalTeam = team };
        var cancellationToken = CancellationToken.None;

        var serviceResult = GenResult<ITeamDeviceService>.Success(_deviceServiceMock.Object);
        _deviceServiceFactoryMock.Setup(x => x.GetServiceAsync(team, dto.SubscriptionId))
            .ReturnsAsync(serviceResult);

        _deviceServiceMock.Setup(s => s.Subscription)
                  .Returns(subscription);

        var updateResult = GenResult<TeamDevice>.Success(device);
        _deviceServiceMock.Setup(x => x.UpdateDeviceAsync(dto.Id, It.IsAny<Name>(), It.IsAny<DescriptionNullable>()))
            .ReturnsAsync(updateResult);

        // Act
        var result = await _handler.Handle(request, cancellationToken);

        // Assert
        result.Succeeded.ShouldBeTrue();
        _deviceServiceFactoryMock.Verify(x => x.GetServiceAsync(team, dto.SubscriptionId), Times.Once);
        _deviceServiceMock.Verify(x => x.UpdateDeviceAsync(dto.Id, It.IsAny<Name>(), It.IsAny<DescriptionNullable>()), Times.Once);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnFailureResult_WhenServiceFailsToRetrieve()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var dto = new DeviceDto
        {
            Id = Guid.NewGuid(),
            Name = "DeviceName",
            Description = "DeviceDescription",
            SubscriptionId = Guid.NewGuid()
        };
        var request = new UpdateDeviceCmd(dto) { PrincipalTeam = team };
        var cancellationToken = CancellationToken.None;

        var serviceResult = GenResult<ITeamDeviceService>.Failure("Service not found");
        _deviceServiceFactoryMock.Setup(x => x.GetServiceAsync(team, dto.SubscriptionId))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _handler.Handle(request, cancellationToken);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldBe("Service not found");
        _deviceServiceFactoryMock.Verify(x => x.GetServiceAsync(team, dto.SubscriptionId), Times.Once);
        _deviceServiceMock.Verify(x => x.UpdateDeviceAsync(It.IsAny<Guid>(), It.IsAny<Name>(), It.IsAny<DescriptionNullable>()), Times.Never);
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
        var dvcToUdate = dvcs.First();
        var subscription = SubscriptionDataFactory.Create(subscriptionId, null, null, null, null, null, 0, dvcs);
        var dto = new DeviceDto
        {
            Id = dvcToUdate.Id,
            Name = "DeviceName",
            Description = "DeviceDescription",
            SubscriptionId = Guid.NewGuid()
        };
        var team = TeamDataFactory.Create(teamId, null, null, [subscription]);

        var request = new UpdateDeviceCmd(dto) { PrincipalTeam = team };

        _deviceServiceFactoryMock.Setup(f => f.GetServiceAsync(team, dto.SubscriptionId))
                 .ReturnsAsync(GenResult<ITeamDeviceService>.NotFoundResult("Service not found"));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeFalse();
    }



}//Cls
