namespace ID.Infrastructure.Tests.Persistence.Repos.Specs.Teams;

public class TeamByIdWithSubscriptionsSpecTests
{
    [Fact]
    public void Constructor_SetsCriteriaCorrectly()
    {
        // Arrange
        var team = TeamDataFactory.Create();

        // Act
        var spec = new TeamByIdWithSubscriptionsSpec(team.Id);
        var criteria = spec.TESTING_GetCriteria().Compile();

        // Assert
        criteria(team).ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public void Constructor_SetsCriteriaCorrectly_FALSE_IfWrongId()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var differentId = Guid.NewGuid();

        // Act
        var spec = new TeamByIdWithSubscriptionsSpec(differentId);
        var criteria = spec.TESTING_GetCriteria().Compile();

        // Assert
        criteria(team).ShouldBeFalse();
    }

    //------------------------------------//

    [Fact]
    public void Constructor_SetsShortCircuitCorrectly()
    {
        // Arrange
        Guid? id = null;

        // Act
        var spec = new TeamByIdWithSubscriptionsSpec(id);

        // Assert
        spec.ShouldShortCircuit().ShouldBeTrue();
    }

    //------------------------------------//
}
