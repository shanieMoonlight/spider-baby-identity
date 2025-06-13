namespace ID.Infrastructure.Tests.Persistence.Repos.Specs.Teams;

public class AllTeamsSpecTests
{
    [Fact]
    public void Constructor_SetsCriteriaCorrectly()
    {
        // Arrange
        var team = TeamDataFactory.Create();

        // Act
        var spec = new AllTeamsSpec();
        var criteria = spec.TESTING_GetCriteria().Compile();

        // Assert
        criteria(team).ShouldBeTrue();
    }
}
