namespace ID.Application.Tests.Features.Account.Cmd.TrustedDevices.Cmd.Trust;

public class TrustDeviceCmdHandlerTests
{
    [Fact]
    public async Task Should_Call_AddAsync_And_Return_Success_When_Service_Succeeds()
    {
        // Arrange
        var user = AppUserDataFactory.Create();
        var userAgent = "ua-1";

        var dto = new TrustDeviceCreateDto(
            DeviceFingerprint: "fp-123",
            DeviceName: "My Device");

        var cmd = new TrustDeviceCmd(dto)
        {
            PrincipalUser = user
        };

        var trustedDevice = TrustedDeviceDataFactory.Create(
            user: user,
            deviceFingerprint: dto.DeviceFingerprint,
            name: dto.DeviceName,
            userAgent: userAgent
        );

        var mockDeviceService = new Mock<IDeviceTrustService<AppUser>>();
        mockDeviceService.Setup(s => s.TrustAsync(
                It.Is<AppUser>(u => u == user),
                It.Is<string>(df => df == dto.DeviceFingerprint),
                It.Is<string>(dn => dn == dto.DeviceName),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<TrustedDevice>.Success(trustedDevice));

        var mockRefreshService = new Mock<IJwtRefreshTokenService<AppUser>>();
        // return a simple GeneratedTokenDto; token itself not asserted here
        mockRefreshService.Setup(r => r.GenerateAndStoreWithDeviceAsync(
                It.IsAny<AppUser>(), It.IsAny<IEnumerable<AuthMethodRef>>(), It.IsAny<TrustedDevice>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GeneratedTokenDto(RefreshTokenDataFactory.Create(user: user), "client-token"));

        var handler = new TrustDeviceCmdHandler(mockDeviceService.Object, mockRefreshService.Object);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        mockDeviceService.Verify(s => s.TrustAsync(
                It.IsAny<AppUser>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.Device.Fingerprint.ShouldBe(trustedDevice.Fingerprint);
    }

    //--------------------------// 

    [Fact]
    public async Task Should_Return_BadRequest_When_Service_Returns_BadRequest()
    {
        // Arrange
        var user = AppUserDataFactory.Create();

        var dto = new TrustDeviceCreateDto(
            DeviceFingerprint: "fp-456",
            DeviceName: "Other Device"
        );

        var cmd = new TrustDeviceCmd(dto)
        {
            PrincipalUser = user
        };

        var errorMsg = "invalid request";

        var mockDeviceService = new Mock<IDeviceTrustService<AppUser>>();
        mockDeviceService.Setup(s => s.TrustAsync(
                It.IsAny<AppUser>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<TrustedDevice>.BadRequestResult(errorMsg));

        var mockRefreshService = new Mock<IJwtRefreshTokenService<AppUser>>();

        var handler = new TrustDeviceCmdHandler(mockDeviceService.Object, mockRefreshService.Object);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.BadRequest.ShouldBeTrue();
        result.Info.ShouldBe(errorMsg);
    }
}
