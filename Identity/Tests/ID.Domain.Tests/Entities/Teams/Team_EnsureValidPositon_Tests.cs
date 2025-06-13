using ID.Tests.Data.Factories;
using Shouldly;

namespace ID.GlobalSettings.Tests.Entities.Teams;

public class Team_EnsureValidPositon_Tests
{
    [Fact]
    public void EnsureValidPosition_ShouldReturnMaxPosition_WhenPositionIsNull()
    {
        // Arrange
        var team = TeamDataFactory.Create(maxPosition: 10);

        // Act
        var result = team.EnsureValidPosition(null);

        // Assert
        result.ShouldBe(10);
    }

    //------------------------------------//     

    [Fact]
    public void EnsureValidPosition_ShouldReturnMinPosition_WhenPositionIsLessThanMinPosition()
    {
        // Arrange
        var team = TeamDataFactory.Create(minPosition: 5);

        // Act
        var result = team.EnsureValidPosition(3);

        // Assert
        result.ShouldBe(5);
    }

    //------------------------------------//     

    [Fact]
    public void EnsureValidPosition_ShouldReturnMaxPosition_WhenPositionIsGreaterThanMaxPosition()
    {
        // Arrange
        var team = TeamDataFactory.Create(maxPosition: 10);

        // Act
        var result = team.EnsureValidPosition(12);

        // Assert
        result.ShouldBe(10);
    }

    //------------------------------------//     

    [Fact]
    public void EnsureValidPosition_ShouldReturnPosition_WhenPositionIsWithinRange()
    {
        // Arrange
        var team = TeamDataFactory.Create(minPosition: 5, maxPosition: 10);

        // Act
        var result = team.EnsureValidPosition(7);

        // Assert
        result.ShouldBe(7);
    }

    //------------------------------------//     

}//Cls