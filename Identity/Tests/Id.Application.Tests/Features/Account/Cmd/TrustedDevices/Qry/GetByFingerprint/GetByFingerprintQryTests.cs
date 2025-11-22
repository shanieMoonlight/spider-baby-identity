using ID.Application.Features.Account.Cmd.TrustedDevices.Qry.GetByName;

namespace ID.Application.Tests.Features.Account.Cmd.TrustedDevices.Qry.GetByFingerprint;

public class GetByFingerprintQryTests
{
    [Fact]
    public void GetTrustedDevicesPageQry_Implements_IIdUserAwareRequest()
    {
        // Arrange
        var command = new GetTrustedDeviceByFingerprintQry("fingerprint");

        // Act & Assert
        command.ShouldBeAssignableTo<IIdUserAwareRequest<AppUser>>();
    }
}
