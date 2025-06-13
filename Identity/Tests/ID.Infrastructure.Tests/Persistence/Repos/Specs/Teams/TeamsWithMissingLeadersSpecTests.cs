namespace ID.Infrastructure.Tests.Persistence.Repos.Specs.Teams;

public class TeamsWithMissingLeadersSpecTests
{
    [Fact]
    public void Constructor_SetsCriteriaCorrectly()
    {
        // Arrange
        var team = TeamDataFactory.Create(leaderId: null);

        // Act
        var spec = new TeamsWithMissingLeadersSpec();
        var criteria = spec.TESTING_GetCriteria().Compile();

        // Assert
        criteria(team).ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public void Constructor_SetsCriteriaCorrectly_FALSE_IfLeaderExists()
    {
        // Arrange
        var team = TeamDataFactory.Create(leaderId: Guid.NewGuid());

        // Act
        var spec = new TeamsWithMissingLeadersSpec();
        var criteria = spec.TESTING_GetCriteria().Compile();

        // Assert
        criteria(team).ShouldBeFalse();
    }

    //------------------------------------//
}
