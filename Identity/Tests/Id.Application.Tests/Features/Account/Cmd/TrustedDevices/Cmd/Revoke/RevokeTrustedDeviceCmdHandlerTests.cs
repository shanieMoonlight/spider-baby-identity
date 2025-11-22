using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Shouldly;
using Microsoft.Extensions.Logging;
using MyResults;
using ID.Domain.Entities.AppUsers;
using ID.Tests.Data.Factories;
using ID.Application.Features.Account.Cmd.TrustedDevices.Cmd.Revoke;

namespace ID.Application.Tests.Features.Account.Cmd.TrustedDevices.Cmd.Revoke;

public class RevokeTrustedDeviceCmdHandlerTests
{
    [Fact]
    public async Task Should_Call_RevokeAsync_And_Return_Success_When_Service_Succeeds()
    {
        // Arrange
        var user = AppUserDataFactory.Create();
        var dto = new RevokeTrustedDeviceDto(Guid.NewGuid());
        var cmd = new RevokeTrustedDeviceCmd(dto)
        {
            PrincipalUser = user
        };

        var mockService = new Mock<Domain.Abstractions.Services.TrustedDevices.ITrustedDeviceService<AppUser>>();
        mockService.Setup(s => s.RevokeAsync(
                It.Is<AppUser>(u => u == user),
                It.Is<Guid>(id => id == dto.DeviceId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(BasicResult.Success());

        var logger = Mock.Of<ILogger<RevokeTrustedDeviceCmdHandler>>();
        var handler = new RevokeTrustedDeviceCmdHandler(mockService.Object, logger);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        mockService.Verify(s => s.RevokeAsync(
                It.IsAny<AppUser>(),
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        result.Succeeded.ShouldBeTrue();
    }

    //--------------------------//

    [Fact]
    public async Task Should_Return_BadRequest_When_Service_Returns_BadRequest()
    {
        // Arrange
        var user = AppUserDataFactory.Create();
        var dto = new RevokeTrustedDeviceDto(Guid.NewGuid());
        var cmd = new RevokeTrustedDeviceCmd(dto)
        {
            PrincipalUser = user
        };

        var mockService = new Mock<Domain.Abstractions.Services.TrustedDevices.ITrustedDeviceService<AppUser>>();
        mockService.Setup(s => s.RevokeAsync(
                It.IsAny<AppUser>(),
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(BasicResult.BadRequestResult("bad"));

        var logger = Mock.Of<ILogger<RevokeTrustedDeviceCmdHandler>>();
        var handler = new RevokeTrustedDeviceCmdHandler(mockService.Object, logger);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.BadRequest.ShouldBeTrue();
        result.Info.ShouldBe("bad");
    }
}
