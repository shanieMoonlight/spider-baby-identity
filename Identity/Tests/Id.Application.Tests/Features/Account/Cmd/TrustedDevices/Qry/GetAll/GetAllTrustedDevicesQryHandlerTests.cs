using ID.Application.Features.Account.Cmd.TrustedDevices.Qry.GetAll;
using ID.Domain.Entities.TrustedDevices;

namespace ID.Application.Tests.Features.Account.Cmd.TrustedDevices.Qry.GetAll;

public class GetAllTrustedDevicesQryHandlerTests
{
    [Fact]
    public async Task Handle_Should_Return_All_Devices()
    {
        // Arrange   
        var userId = Guid.NewGuid();
        var devices = new HashSet<TrustedDevice>();
        for (int i = 0; i < 5; i++)
        {
            devices.Add(TrustedDeviceDataFactory.Create(userId: userId));
        }
        var user = AppUserDataFactory.Create(id: userId, trustedDevices: devices);

        var qry = new GetAllTrustedDevicesQry()
        {
            PrincipalUser = user
        };

        var handler = new GetAllTrustedDevicesQryHandler();

        // Act
        var result = await handler.Handle(qry, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.Count().ShouldBe(devices.Count);
    }
}
