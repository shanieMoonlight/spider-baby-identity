using ID.Application.Features.Account.Cmd.PwdForgot;
using ID.Application.Mediatr.CqrsAbs;
using Shouldly;

namespace ID.Application.Tests.Features.Account.Cmd.PwdForgot;

public class ForgotPwdCmdTests
{
    [Fact]
    public void Implements_IIdPrincipalInfoRequest()
    {
        // Arrange
        var dto = new ForgotPwdDto();
        var command = new ForgotPwdCmd(dto);

        // Act & Assert
        Assert.IsType<IIdPrincipalInfoRequest>(command, exactMatch: false);

        // Act & Assert
        command.ShouldBeAssignableTo<IIdPrincipalInfoRequest>();
    }
}
