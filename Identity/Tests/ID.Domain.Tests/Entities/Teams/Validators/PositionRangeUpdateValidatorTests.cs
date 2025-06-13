using ID.Domain.Entities.Teams.Validators;
using ID.Domain.Entities.Teams.ValueObjects;
using ID.Tests.Data.Factories;
using Shouldly;
using Xunit;

namespace ID.Domain.Tests.Entities.Teams.Validators;

public class PositionRangeUpdateValidatorTests
{
    [Fact]
    public void Validate_ShouldAlwaysReturnSuccess()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var newPositionRange = TeamPositionRange.Create(1, 10);

        // Act
        var result = TeamValidators.PositionRangeUpdate.Validate(team, newPositionRange);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Team.ShouldBe(team);
        result.Value.NewPositionRange.ShouldBe(newPositionRange);
    }

    //------------------------// 


    [Theory]
    [InlineData(1, 5)]
    [InlineData(10, 20)]
    [InlineData(0, 100)]
    public void Validate_WithDifferentPositionRanges_ShouldAlwaysReturnSuccess(int min, int max)
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var newPositionRange = TeamPositionRange.Create(min, max);

        // Act
        var result = TeamValidators.PositionRangeUpdate.Validate(team, newPositionRange);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Team.ShouldBe(team);
        result.Value.NewPositionRange.ShouldBe(newPositionRange);
    }

    //------------------------// 


    [Fact]
    public void Token_ShouldStoreTeamAndPositionRangeCorrectly()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var newPositionRange = TeamPositionRange.Create(1, 10);

        // Act
        var result = TeamValidators.PositionRangeUpdate.Validate(team, newPositionRange);

        // Assert
        result.Succeeded.ShouldBeTrue();
        var token = result.Value!;
        token.Team.ShouldBe(team);
        token.NewPositionRange.ShouldBe(newPositionRange);
    }

    //------------------------// 


    [Fact]
    public void Token_ShouldImplementIValidationToken()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var newPositionRange = TeamPositionRange.Create(1, 10);

        // Act
        var result = TeamValidators.PositionRangeUpdate.Validate(team, newPositionRange);

        // Assert
        result.Succeeded.ShouldBeTrue();
        var token = result.Value!;
        token.ShouldBeAssignableTo<IValidationToken>();
    }

    //------------------------// 

}//Cls
