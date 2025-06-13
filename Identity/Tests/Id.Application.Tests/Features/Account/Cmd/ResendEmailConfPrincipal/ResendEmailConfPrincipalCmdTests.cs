using Shouldly;
using ID.Application.Features.Account.Cmd.ResendEmailConfirmationPrincipal;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Tests.Features.Account.Cmd.ResendEmailConfPrincipal;

public class ResendEmailConfPrincipalCmdTests
{
    [Fact]
    public void Implements_IIdUserAwareRequest()
    {
        // Arrange
        var command = new ResendEmailConfirmationPrincipalCmd();

        // Act & Assert
        Assert.IsType<IIdUserAwareRequest<AppUser>>(command, exactMatch: false);

        // Act & Assert
        command.ShouldBeAssignableTo<IIdUserAwareRequest<AppUser>>();
    }
}
