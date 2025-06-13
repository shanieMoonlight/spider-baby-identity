using ID.Application.Features.Account.Cmd.AddMntcMember;
using Shouldly;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Tests.Features.Account.Cmd.AddMntc;

public class AddMntcMemberCmdTests
{
    [Fact]
    public void AddMntcMemberCmd_Implements_IIdUserAwareRequest()
    {
        //NOt implementing IIdUserAndTeamAwareRequest because it tight be a SuperMember adding to the MntcTeam in rare cases
        // Arrange
        var dto = new AddMntcMemberDto();
        var command = new AddMntcMemberCmd(dto);

        // Act & Assert
        command.ShouldBeAssignableTo<IIdUserAwareRequest<AppUser>>();
    }
}
