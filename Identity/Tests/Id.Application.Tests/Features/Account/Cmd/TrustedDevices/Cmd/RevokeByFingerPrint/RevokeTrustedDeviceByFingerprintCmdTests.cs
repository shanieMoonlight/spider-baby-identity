using ID.Application.Features.Account.Cmd.TrustedDevices.Cmd.Revoke;

namespace ID.Application.Tests.Features.Account.Cmd.TrustedDevices.Cmd.RevokeByFingerPrint;

public class RevokeTrustedDeviceByFingerprintCmdTests
{
    [Fact]
    public void GetTrustedDevicesPageQry_Implements_IIdUserAwareRequest()
    {
        // Arrange
        var dto = new RevokeTrustedDeviceDto(Guid.NewGuid());
        var command = new RevokeTrustedDeviceCmd(dto);

        // Act & Assert
        command.ShouldBeAssignableTo<IIdUserAwareRequest<AppUser>>();
    }
}
