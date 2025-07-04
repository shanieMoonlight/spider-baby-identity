using ID.Domain.Entities.Teams;
using ID.Infrastructure.Persistance.EF.Repos.Specs.Teams;
using ID.Tests.Data.Factories;
using Shouldly;

namespace ID.Infrastructure.Tests.Persistence.Repos.Specs.Teams;

public class CustomerTeamsByNameSpecTests
{
    [Fact]
    public void Constructor_SetsCriteriaCorrectly()
    {
        // Arrange
        var team = TeamDataFactory.Create(name: "TestTeam", teamType: TeamType.customer);

        // Act
        var spec = new CustomerTeamsByNameSpec(team.Name);
        var criteria = spec.TESTING_GetCriteria().Compile();

        // Assert
        criteria(team).ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public void Constructor_SetsCriteriaCorrectly_FALSE_IfWrongName()
    {
        // Arrange
        var team = TeamDataFactory.Create(name: "TestTeam", teamType: TeamType.customer);
        var differentName = "DifferentTeam";

        // Act
        var spec = new CustomerTeamsByNameSpec(differentName);
        var criteria = spec.TESTING_GetCriteria().Compile();

        // Assert
        criteria(team).ShouldBeFalse();
    }

    //------------------------------------//

    [Fact]
    public void Constructor_SetsShortCircuitCorrectly()
    {
        // Arrange
        string? name = null;

        // Act
        var spec = new CustomerTeamsByNameSpec(name);

        // Assert
        spec.ShouldShortCircuit().ShouldBeTrue();
    }

    //------------------------------------//
}
