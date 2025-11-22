using ID.Application.Features.Account.Cmd.TrustedDevices.Cmd.RevokeByFingerPrint;

namespace ID.Application.Tests.Features.Account.Cmd.TrustedDevices.RevokeByFingerPrint;

public class RevokeTrustedDeviceByFingerprintCmdHandlerTests
{
    [Fact]
    public async Task Should_Call_RevokeAsync_And_Return_Success_When_Service_Succeeds()
    {
        // Arrange
        var user = AppUserDataFactory.Create();
        var dto = new RevokeTrustedDeviceByFingerprintDto("fp-123");
        var cmd = new RevokeTrustedDeviceByFingerprintCmd(dto)
        {
            PrincipalUser = user
        };

        var mockService = new Mock<ID.Domain.Abstractions.Services.TrustedDevices.ITrustedDeviceService<AppUser>>();
        mockService.Setup(s => s.RevokeAsync(
                It.Is<AppUser>(u => u == user),
                It.Is<string>(f => f == dto.DeviceFingerprint),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(BasicResult.Success());

        var logger = Mock.Of<ILogger<RevokeTrustedDeviceByFingerPrintCmdHandler>>();
        var handler = new RevokeTrustedDeviceByFingerPrintCmdHandler(mockService.Object, logger);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        mockService.Verify(s => s.RevokeAsync(
                It.IsAny<AppUser>(),
                It.IsAny<string>(),
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
        var dto = new RevokeTrustedDeviceByFingerprintDto("fp-456");
        var cmd = new RevokeTrustedDeviceByFingerprintCmd(dto)
        {
            PrincipalUser = user
        };

        var mockService = new Mock<ID.Domain.Abstractions.Services.TrustedDevices.ITrustedDeviceService<AppUser>>();
        mockService.Setup(s => s.RevokeAsync(
                It.IsAny<AppUser>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(BasicResult.BadRequestResult("bad"));

        var logger = Mock.Of<ILogger<RevokeTrustedDeviceByFingerPrintCmdHandler>>();
        var handler = new RevokeTrustedDeviceByFingerPrintCmdHandler(mockService.Object, logger);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.BadRequest.ShouldBeTrue();
        result.Info.ShouldBe("bad");
    }
}
