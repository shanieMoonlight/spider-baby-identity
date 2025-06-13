using Shouldly;
using ID.Application.Features.Account.Cmd.ConfirmEmailWithPwd;
using ID.Application.Mediatr.CqrsAbs;

namespace ID.Application.Tests.Features.Account.Cmd.ConfirmEmailWithPwd;

public class ConfirmEmailWithPwdTCmdests
{
    [Fact]
    public void ConfirmEmailCmdTests_Implements_IIdUserAndTeamAwareRequest()
    {
        // Arrange
        var command = new ConfirmEmailWithPwdCmd(
            new ConfirmEmailWithPwdDto(new Guid(), "algfdflkgl", "algfdflkgl", "algfdflkgl")
        );

        // Act & Assert
        Assert.IsType<IIdPrincipalInfoRequest>(command, exactMatch: false);

        // Act & Assert
        command.ShouldBeAssignableTo<IIdPrincipalInfoRequest>();
    }
}
