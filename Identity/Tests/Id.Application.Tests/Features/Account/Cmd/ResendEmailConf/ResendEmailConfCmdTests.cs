using Shouldly;
using ID.Application.Features.Account.Cmd.ResendEmailConfirmation;
using ID.Application.Mediatr.CqrsAbs;

namespace ID.Application.Tests.Features.Account.Cmd.ResendEmailConf;

public class ResendEmailConfCmdTests
{
    [Fact]
    public void Implements_IIdPrincipalInfoRequest()
    {
        // Arrange
        var dto = new ResendEmailConfirmationDto();
        var command = new ResendEmailConfirmationCmd(dto);

        // Act & Assert
        Assert.IsType<IIdPrincipalInfoRequest>(command, exactMatch: false);

        // Act & Assert
        command.ShouldBeAssignableTo<IIdPrincipalInfoRequest>();
    }
}
