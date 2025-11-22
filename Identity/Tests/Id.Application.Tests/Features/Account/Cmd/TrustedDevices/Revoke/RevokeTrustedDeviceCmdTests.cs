using ID.Application.Features.Account.Cmd.TrustedDevices.Cmd.Revoke;
using ID.Application.Features.Account.Cmd.TrustedDevices.Qry.GetByName;

namespace ID.Application.Tests.Features.Account.Cmd.TrustedDevices.Revoke;

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
