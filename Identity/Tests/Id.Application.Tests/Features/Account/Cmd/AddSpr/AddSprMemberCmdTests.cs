using Shouldly;
using ID.Application.Features.Account.Cmd.AddSprMember;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Tests.Features.Account.Cmd.AddSpr;

public class AddSprMemberCmdTests
{
    [Fact]
    public void AddSprMemberCmd_Implements_IIdUserAndTeamAwareRequest()
    {
        // Arrange
        var dto = new AddSprMemberDto();
        var command = new AddSprMemberCmd(dto);

        // Act & Assert
        Assert.IsType<IIdUserAndTeamAwareRequest<AppUser>>(command, exactMatch: false);

        // Act & Assert
        command.ShouldBeAssignableTo<IIdUserAndTeamAwareRequest<AppUser>>();
    }
}
