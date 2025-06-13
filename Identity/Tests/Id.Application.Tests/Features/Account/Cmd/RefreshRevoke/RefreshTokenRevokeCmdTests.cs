using ID.Application.Features.Account.Cmd.RefreshTokenRevoke;
using ID.Application.Mediatr.Cqrslmps.Commands;
using ID.Domain.Entities.AppUsers;
using Shouldly;

namespace ID.Application.Tests.Features.Account.Cmd.RefreshRevoke;

public class RefreshTokenRevokeCmdTests
{
    [Fact]
    public void Implements_IIdPrincipalInfoRequestt()
    {
        // Arrange
        var command = new RefreshTokenRevokeCmd();

        // Act & Assert
        Assert.IsType<AIdUserAwareCommand<AppUser>>(command, exactMatch: false);

        // Act & Assert
        command.ShouldBeAssignableTo<AIdUserAwareCommand<AppUser>>();
    }
}
