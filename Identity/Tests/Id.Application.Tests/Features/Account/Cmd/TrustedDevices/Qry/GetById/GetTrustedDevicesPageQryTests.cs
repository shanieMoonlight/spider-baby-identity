using ID.Application.Features.Account.Cmd.TrustedDevices.Qry.GetById;

namespace ID.Application.Tests.Features.Account.Cmd.TrustedDevices.Qry.GetById;

public class GetTrustedDeviceByIdQryTests
{
    [Fact]
    public void GetTrustedDeviceByIdQry_Implements_IIdUserAwareRequest()
    {
        //NOt implementing IIdUserAndTeamAwareRequest because it tight be a SuperMember adding to the MntcTeam in rare cases
        // Arrange
        var command = new GetTrustedDeviceByIdQry(Guid.NewGuid());

        // Act & Assert
        command.ShouldBeAssignableTo<IIdUserAwareRequest<AppUser>>();
    }
}
