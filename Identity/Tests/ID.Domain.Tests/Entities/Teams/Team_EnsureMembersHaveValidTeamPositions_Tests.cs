using ID.Domain.Entities.AppUsers;
using ID.Tests.Data.Factories;
using Shouldly;

namespace ID.GlobalSettings.Tests.Entities.Teams;

public class Team_EnsureMembersHaveValidTeamPositions_Tests
{

    //------------------------------------//

    [Fact]
    public void EnsureMembersHaveValidTeamPositions_ShouldUpdateMemberPositions()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var minPosition = 5;
        var maxPosition = 15;
        var member1 = AppUserDataFactory.Create(teamId, teamPosition: minPosition - 1);
        var member2 = AppUserDataFactory.Create(teamId, teamPosition: maxPosition + 1);
        var member3 = AppUserDataFactory.Create(teamId, teamPosition: (minPosition + maxPosition) / 2);

        var members = new HashSet<AppUser> { member1, member2, member3 };
        var team = TeamDataFactory.Create(id: teamId, members: members, minPosition: minPosition, maxPosition: maxPosition);

        // Act
        team.EnsureMembersHaveValidTeamPositions();

        // Assert
        member1.TeamPosition.ShouldBe(team.MinPosition);
        member2.TeamPosition.ShouldBe(team.MaxPosition);
        member3.TeamPosition.ShouldBe((team.MinPosition + team.MaxPosition) / 2);
    }

    //------------------------------------//
}

