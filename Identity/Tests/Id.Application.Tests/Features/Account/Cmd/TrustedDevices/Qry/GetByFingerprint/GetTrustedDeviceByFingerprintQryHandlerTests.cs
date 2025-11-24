using ID.Application.Features.Account.TrustedDevices.Qry.GetByFingerprint;

namespace ID.Application.Tests.Features.Account.Cmd.TrustedDevices.Qry.GetByFingerprint;

public class GetTrustedDeviceByFingerprintQryHandlerTests
{
    [Fact]
    public async Task Handle_Should_Return_NotFound_When_Not_Found()
    {
        // Arrange
        var user = AppUserDataFactory.Create();
        var qry = new GetTrustedDeviceByFingerprintQry("nope") { PrincipalUser = user };
        var handler = new GetTrustedDeviceByFingerprintQryHandler();

        // Act
        var result = await handler.Handle(qry, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.NotFound.ShouldBeTrue();
    }

    //--------------------------//

    [Fact]
    public async Task Handle_Should_Return_Device_When_Found()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var device = TrustedDeviceDataFactory.Create(userId: userId, deviceFingerprint: "fp-1");
        var user = AppUserDataFactory.Create(id: userId, trustedDevices: [device]);

        var qry = new GetTrustedDeviceByFingerprintQry("fp-1") { PrincipalUser = user };
        var handler = new GetTrustedDeviceByFingerprintQryHandler();

        // Act
        var result = await handler.Handle(qry, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.Fingerprint.ShouldBe("fp-1");
    }
}
