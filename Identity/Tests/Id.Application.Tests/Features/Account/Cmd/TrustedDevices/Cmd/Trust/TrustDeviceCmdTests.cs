namespace ID.Application.Tests.Features.Account.Cmd.TrustedDevices.Cmd.Trust;

public class TrustDeviceCmdTests
{
    [Fact]
    public void GetTrustedDevicesPageQry_Implements_IIdUserAwareRequest()
    {
        // Arrange
        var dto = new TrustDeviceCreateDto("fp", "name");
        var command = new TrustDeviceCmd(dto);

        // Act & Assert
        command.ShouldBeAssignableTo<IIdUserAwareRequest<AppUser>>();
    }
}
