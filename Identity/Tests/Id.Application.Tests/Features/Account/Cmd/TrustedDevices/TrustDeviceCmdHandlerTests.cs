using ID.Application.JWT;
using ID.Domain.Claims.AuthMethods;

namespace ID.Application.Tests.Features.Account.Cmd.TrustedDevices;

public class TrustDeviceCmdHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsFailure_When_DeviceTrustService_Fails()
    {
        // Arrange
        var deviceSvc = new Mock<IDeviceTrustService<AppUser>>();
        var tknSvc = new Mock<IJwtRefreshTokenService<AppUser>>();

        var handler = new TrustDeviceCmdHandler(deviceSvc.Object, tknSvc.Object);

        var dto = new TrustDeviceCreateDto("fp-1", "dev1");
        var cmd = new TrustDeviceCmd(dto);
        var user = AppUserDataFactory.Create();
        cmd.PrincipalUser = user;

        var failure = GenResult<TrustedDevice>.BadRequestResult("fail");
        deviceSvc
            .Setup(s => s.TrustAsync(user, dto.DeviceFingerprint, dto.DeviceName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(failure);

        // Act
        var res = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        res.ShouldNotBeNull();
        res.Succeeded.ShouldBeFalse();
        res.Info.ShouldBe(failure.Info);

        // Ensure token service not called
        tknSvc.Verify(x => x.GenerateTokenWithDeviceAsync(It.IsAny<AppUser>(), It.IsAny<IEnumerable<AuthMethodRef>>(), It.IsAny<TrustedDevice>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    //--------------------//

    [Fact]
    public async Task Handle_ReturnsSuccess_And_GeneratesToken_When_DeviceTrusts()
    {
        // Arrange
        var deviceSvc = new Mock<IDeviceTrustService<AppUser>>();
        var tknSvc = new Mock<IJwtRefreshTokenService<AppUser>>();

        var handler = new TrustDeviceCmdHandler(deviceSvc.Object, tknSvc.Object);

        var dto = new TrustDeviceCreateDto("fp-2", "dev2");
        var cmd = new TrustDeviceCmd(dto);
        var user = AppUserDataFactory.Create();
        cmd.PrincipalUser = user;

        var device = TrustedDeviceDataFactory.Create(user: user, deviceFingerprint: dto.DeviceFingerprint);
        deviceSvc
            .Setup(s => s.TrustAsync(user, dto.DeviceFingerprint, dto.DeviceName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<TrustedDevice>.Success(device));

        var createdToken = RefreshTokenDataFactory.Create(user: user, trustedDevice: device);
        tknSvc
            .Setup(s => s.GenerateTokenWithDeviceAsync(
                It.Is<AppUser>(u => u.Id == user.Id),
                It.Is<IEnumerable<AuthMethodRef>>(l => l.Count() == 1 && l.First() == AuthMethodRef.mfa),
                It.Is<TrustedDevice>(d => d == device),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdToken);

                //new List<AuthMethodRef>() { AuthMethodRef.mfa },
        // Act
        var res = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        res.ShouldNotBeNull();
        res.Succeeded.ShouldBeTrue();
        res.Value.ShouldNotBeNull();
        res.Value.RefreshToken.ShouldBe(createdToken.Payload);
        res.Value.Device.ShouldNotBeNull();
        res.Value.Device.Fingerprint.ShouldBe(dto.DeviceFingerprint);

        deviceSvc.Verify(s => s.TrustAsync(user, dto.DeviceFingerprint, dto.DeviceName, It.IsAny<CancellationToken>()), Times.Once);
        tknSvc.Verify(s => s.GenerateTokenWithDeviceAsync(user, It.IsAny<IEnumerable<AuthMethodRef>>(), device, It.IsAny<CancellationToken>()), Times.Once);
    }
}
