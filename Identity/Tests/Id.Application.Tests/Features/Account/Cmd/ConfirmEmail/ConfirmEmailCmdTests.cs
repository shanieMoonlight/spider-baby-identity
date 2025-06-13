using Shouldly;
using ID.Application.Features.Account.Cmd.ConfirmEmail;
using ID.Application.Mediatr.CqrsAbs;

namespace ID.Application.Tests.Features.Account.Cmd.ConfirmEmail;

public class ConfirmEmailCmdTests
{
    [Fact]
    public void Implements_IIdPrincipalInfoRequestt()
    {
        // Arrange
        var command = new ConfirmEmailCmd(new ConfirmEmailDto(new Guid(), "algfdflkgl"));

        // Act & Assert
        Assert.IsType<IIdPrincipalInfoRequest>(command, exactMatch: false);

        // Act & Assert
        command.ShouldBeAssignableTo<IIdPrincipalInfoRequest>();
    }
}
