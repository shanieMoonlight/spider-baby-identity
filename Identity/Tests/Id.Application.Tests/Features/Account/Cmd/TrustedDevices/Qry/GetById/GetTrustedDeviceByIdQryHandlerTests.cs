using ID.Application.Features.Account.Cmd.TrustedDevices.Qry.GetById;

namespace ID.Application.Tests.Features.Account.Cmd.TrustedDevices.Qry.GetById;

public class GetTrustedDeviceByIdQryHandlerTests
{
    [Fact]
    public async Task Handle_Should_Return_NotFound_When_Device_Does_Not_Exist()
    {
        // Arrange
        var user = AppUserDataFactory.Create();
        var qry = new GetTrustedDeviceByIdQry(Guid.NewGuid()) { PrincipalUser = user };
        var handler = new GetTrustedDeviceByIdQryHandler();

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
        // Arrange#
        var userId = Guid.NewGuid();
        var device = TrustedDeviceDataFactory.Create(userId: userId);
        var user = AppUserDataFactory.Create(id: userId, trustedDevices: [device]);

        var qry = new GetTrustedDeviceByIdQry(device.Id) { PrincipalUser = user };
        var handler = new GetTrustedDeviceByIdQryHandler();

        // Act
        var result = await handler.Handle(qry, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.Id.ShouldBe(device.Id);
    }


}
