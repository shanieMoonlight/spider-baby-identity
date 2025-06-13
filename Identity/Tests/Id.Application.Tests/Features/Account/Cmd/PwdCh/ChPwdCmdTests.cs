using Shouldly;
using ID.Application.Features.Account.Cmd.PwdChange;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Tests.Features.Account.Cmd.PwdCh;

public class ChPwdCmdTests
{
    [Fact]
    public void Implements_IIdUserAwareRequest()
    {
        // Arrange
        var dto = new ChPwdDto();
        var command = new ChPwdCmd(dto);

        // Act & Assert
        Assert.IsType<IIdUserAwareRequest<AppUser>>(command, exactMatch: false);

        // Act & Assert
        command.ShouldBeAssignableTo<IIdUserAwareRequest<AppUser>>();
    }
}
