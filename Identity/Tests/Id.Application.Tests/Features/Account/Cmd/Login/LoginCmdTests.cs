using Shouldly;
using ID.Application.Features.Account.Cmd.Login;
using ID.Application.Mediatr.CqrsAbs;

namespace ID.Application.Tests.Features.Account.Cmd.Login;

public class LoginCmdTests
{
    [Fact]
    public void Implements_IIdPrincipalInfoRequestt()
    {
        // Arrange
        var dto = new LoginDto()
        {
            UserId = new Guid(),
            Username = "algfdflkgl",
            Email = "algfdflkgl",
            Password = "algfdflkgl",
            DeviceId = "algfdflkgl",
        };
        var command = new LoginCmd(dto);

        // Act & Assert
        Assert.IsType<IIdPrincipalInfoRequest>(command, exactMatch: false);

        // Act & Assert
        command.ShouldBeAssignableTo<IIdPrincipalInfoRequest>();
    }
}
