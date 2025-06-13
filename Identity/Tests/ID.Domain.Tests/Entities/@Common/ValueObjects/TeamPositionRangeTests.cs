using ClArch.ValueObjects.Exceptions;
using ID.Domain.Entities.Teams.ValueObjects;
using Shouldly;

namespace ID.Domain.Tests.Entities.@Common.ValueObjects;

public class TeamPositionRangeTests()
{

    //------------------------------------//  

    [Fact]
    public void Create_ShouldReturnTeamPositionRange_WhenValidPositions()
    {
        // Arrange
        int minPosition = 1;
        int maxPosition = 5;

        // Act
        var result = TeamPositionRange.Create(minPosition, maxPosition);

        // Assert
        result.Value.Min.ShouldBe(minPosition);
        result.Value.Max.ShouldBe(maxPosition);
    }

    //------------------------------------//  

    [Fact]
    public void Create_ShouldThrowInvalidPropertyException_WhenMinPositionIsGreaterThanMaxPosition()
    {
        // Arrange
        int minPosition = 5;
        int maxPosition = 1;

        // Act & Assert
        Should.Throw<InvalidPropertyException>(() => TeamPositionRange.Create(minPosition, maxPosition));
    }

    //------------------------------------//  

}//Cls