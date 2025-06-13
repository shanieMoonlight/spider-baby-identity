using ID.Application.Features.MemberMgmt.Cmd.UpdateLeaderMntc;
using ID.Application.Features.Teams;
using ID.Application.Mediatr.Cqrslmps.Commands;
using ID.Domain.Entities.AppUsers;
using Shouldly;

namespace ID.Application.Tests.Features.Teams.Cmd.UpdateLeader;

public class UpdateTeamLeaderCmdTests
{
    [Fact]
    public void AddMntcMemberCmd_Implements_IPrincipalInfoRequest()
    {
        // Arrange
        var command = new UpdateTeamLeaderCmd(new UpdateTeamLeaderDto(Guid.NewGuid(), Guid.NewGuid()));

        // Act & Assert
        command.ShouldBeAssignableTo<AIdUserAndTeamAwareCommand<AppUser, TeamDto>>();
    }
}
