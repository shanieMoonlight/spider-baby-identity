using ID.Application.Features.Account.TrustedDevices.Cmd.Revoke;

namespace ID.Application.Tests.Features.Account.Cmd.TrustedDevices.Cmd.Revoke;

public class RevokeTrustedDeviceCmdTests
{
    [Fact]
    public void RevokeTrustedDeviceCmd_Implements_IIdUserAwareRequest()
    {
        // Arrange
        var dto = new RevokeTrustedDeviceDto(Guid.NewGuid());
        var command = new RevokeTrustedDeviceCmd(dto);

        // Act & Assert
        command.ShouldBeAssignableTo<IIdUserAwareRequest<AppUser>>();
    }
}
