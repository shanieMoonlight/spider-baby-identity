using ID.Domain.Entities.Teams;
using ID.Infrastructure.Persistance.EF.Repos.Specs.Teams;
using ID.Tests.Data.Factories;
using Shouldly;
using Xunit;

namespace ID.Infrastructure.Tests.Persistence.Repos.Specs.Teams;

public class MntcTeamWithMembersSpecTests
{
    [Fact]
    public void Constructor_SetsCriteriaCorrectly()
    {
        // Arrange
        var team = TeamDataFactory.Create(teamType: TeamType.maintenance);

        // Act
        var spec = new MntcTeamWithMembersSpec(1000);
        var criteria = spec.TESTING_GetCriteria().Compile();

        // Assert
        criteria(team).ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public void Constructor_SetsCriteriaCorrectly_FALSE_IfWrongType()
    {
        // Arrange
        var team = TeamDataFactory.Create(teamType: TeamType.customer);

        // Act
        var spec = new MntcTeamWithMembersSpec(1000);
        var criteria = spec.TESTING_GetCriteria().Compile();

        // Assert
        criteria(team).ShouldBeFalse();
    }

    //------------------------------------//
}
