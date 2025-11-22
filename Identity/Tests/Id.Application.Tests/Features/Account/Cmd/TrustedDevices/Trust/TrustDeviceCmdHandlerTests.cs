using ID.Application.Features.Account.Cmd.TrustedDevices.Cmd.Trust;
using ID.Domain.Entities.TrustedDevices;
using ID.Domain.Entities.TrustedDevices.ValueObjects;

namespace ID.Application.Tests.Features.Account.Cmd.TrustedDevices.Trust;

public class TrustDeviceCmdHandlerTests
{
    [Fact]
    public async Task Should_Call_AddAsync_And_Return_Success_When_Service_Succeeds()
    {
        // Arrange
        var user = AppUserDataFactory.Create();

        var dto = new TrustDeviceCreateDto(
            DeviceFingerprint: "fp-123",
            DeviceName: "My Device",
            UserAgent: "ua-1",
            TrustDays: null
        );

        var cmd = new TrustDeviceCmd(dto)
        {
            PrincipalUser = user
        };

        var trustedDevice = TrustedDeviceDataFactory.Create(
            user: user,
            deviceFingerprint: dto.DeviceFingerprint,
            name: dto.DeviceName,
            userAgent: dto.UserAgent
        );

        var mockService = new Mock<ID.Domain.Abstractions.Services.TrustedDevices.ITrustedDeviceService<AppUser>>();
        mockService.Setup(s => s.AddAsync(
                It.Is<AppUser>(u => u == user),
                It.Is<DeviceFingerprint>(df => df.Value == dto.DeviceFingerprint),
                It.Is<DeviceName>(dn => dn.Value == dto.DeviceName),
                It.Is<UserAgent>(ua => ua.Value == dto.UserAgent),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<TrustedDevice>.Success(trustedDevice));

        var handler = new TrustDeviceCmdHandler(mockService.Object);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        mockService.Verify(s => s.AddAsync(
                It.IsAny<AppUser>(),
                It.IsAny<DeviceFingerprint>(),
                It.IsAny<DeviceName>(),
                It.IsAny<UserAgent>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.DeviceFingerprint.ShouldBe(trustedDevice.DeviceFingerprint);
    }

    //--------------------------// 

    [Fact]
    public async Task Should_Return_BadRequest_When_Service_Returns_BadRequest()
    {
        // Arrange
        var user = AppUserDataFactory.Create();

        var dto = new TrustDeviceCreateDto(
            DeviceFingerprint: "fp-456",
            DeviceName: "Other Device",
            UserAgent: null,
            TrustDays: null
        );

        var cmd = new TrustDeviceCmd(dto)
        {
            PrincipalUser = user
        };

        var errorMsg = "invalid request";

        var mockService = new Mock<ID.Domain.Abstractions.Services.TrustedDevices.ITrustedDeviceService<AppUser>>();
        mockService.Setup(s => s.AddAsync(
                It.IsAny<AppUser>(),
                It.IsAny<DeviceFingerprint>(),
                It.IsAny<DeviceName>(),
                It.IsAny<UserAgent>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<TrustedDevice>.BadRequestResult(errorMsg));

        var handler = new TrustDeviceCmdHandler(mockService.Object);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.BadRequest.ShouldBeTrue();
        result.Info.ShouldBe(errorMsg);
    }
}//Cls
