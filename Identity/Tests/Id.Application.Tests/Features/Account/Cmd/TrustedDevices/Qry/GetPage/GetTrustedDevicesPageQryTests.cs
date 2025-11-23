using ID.Application.Features.Account.TrustedDevices.Qry.GetPage;
using Pagination;

namespace ID.Application.Tests.Features.Account.Cmd.TrustedDevices.Qry.GetPage;

public class GetTrustedDevicesPageQryTests
{
    [Fact]
    public void GetTrustedDevicesPageQry_Implements_IIdUserAwareRequest()
    {
        //NOt implementing IIdUserAndTeamAwareRequest because it tight be a SuperMember adding to the MntcTeam in rare cases
        // Arrange
        var request = new PagedRequest();
        var command = new GetTrustedDevicesPageQry(request);

        // Act & Assert
        command.ShouldBeAssignableTo<IIdUserAwareRequest<AppUser>>();
    }
}
