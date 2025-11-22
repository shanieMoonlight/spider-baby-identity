using ID.Application.Features.Account.Cmd.TrustedDevices.Qry.GetAll;

namespace ID.Application.Tests.Features.Account.Cmd.TrustedDevices.Qry.GetAll;

public class GetAllTrustedDevicesQryTests
{
    [Fact]
    public void GetAllTrustedDevicesQry_Implements_IIdUserAwareRequest()
    {
        // Arrange
        var command = new GetAllTrustedDevicesQry();

        // Act & Assert
        command.ShouldBeAssignableTo<IIdUserAwareRequest<AppUser>>();
    }
}
