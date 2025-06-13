using ID.Domain.Entities.Teams.Events;
using ID.Domain.Entities.Teams.Validators;
using ID.Domain.Entities.Teams.ValueObjects;
using ID.Tests.Data.Factories;
using Shouldly;

namespace ID.Domain.Tests.Entities.Teams;

public class Team_UpdatePositionRange_Tests
{
    [Fact]
    public void UpdatePositionRange_WithValidToken_ShouldUpdatePositionsAndRaiseEvent()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var member = AppUserDataFactory.Create(teamId: teamId);
        var team = TeamDataFactory.Create(id: teamId, minPosition: 1, maxPosition: 5, members: [member]);
        var newPositionRange = TeamPositionRange.Create(10, 20);
        
        var validationResult = TeamValidators.PositionRangeUpdate.Validate(team, newPositionRange);
        validationResult.Succeeded.ShouldBeTrue();
        var token = validationResult.Value!;

        // Act
        var result = team.UpdatePositionRange(token);

        // Assert
        result.ShouldBe(team); // Should return same instance
        team.MinPosition.ShouldBe(10);
        team.MaxPosition.ShouldBe(20);
        
        // Should raise domain event
        var domainEvents = team.GetDomainEvents();
        domainEvents.ShouldNotBeEmpty();
        domainEvents.ShouldContain(e => e is TeamPositionRangeUpdatedDomainEvent);
        
        var positionEvent = domainEvents.OfType<TeamPositionRangeUpdatedDomainEvent>().First();
        positionEvent.TeamId.ShouldBe(team.Id);
        positionEvent.Min.ShouldBe(10);
        positionEvent.Max.ShouldBe(20);
    }



    //------------------------//   


    [Theory]
    [InlineData(0, 10)]
    [InlineData(5, 15)]
    [InlineData(1, 100)]
    public void UpdatePositionRange_WithDifferentRanges_ShouldUpdateCorrectly(int min, int max)
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var member = AppUserDataFactory.Create(teamId: teamId);
        var team = TeamDataFactory.Create(id: teamId, members: [member]);
        var newPositionRange = TeamPositionRange.Create(min, max);
        
        var validationResult = TeamValidators.PositionRangeUpdate.Validate(team, newPositionRange);
        validationResult.Succeeded.ShouldBeTrue();
        var token = validationResult.Value!;

        // Act
        team.UpdatePositionRange(token);

        // Assert
        team.MinPosition.ShouldBe(min);
        team.MaxPosition.ShouldBe(max);
    }


    //------------------------//   

    [Fact]
    public void UpdatePositionRange_ShouldPreserveTokenData()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var member = AppUserDataFactory.Create(teamId: teamId);
        var team = TeamDataFactory.Create(id: teamId, members: [member]);
        var newPositionRange = TeamPositionRange.Create(1, 10);
        
        var validationResult = TeamValidators.PositionRangeUpdate.Validate(team, newPositionRange);
        var token = validationResult.Value!;

        // Verify token data before method call
        token.Team.ShouldBe(team);
        token.NewPositionRange.ShouldBe(newPositionRange);

        // Act
        team.UpdatePositionRange(token);

        // Assert - token should still have correct data
        token.Team.ShouldBe(team);
        token.NewPositionRange.ShouldBe(newPositionRange);
    }


    //------------------------//   

    [Fact]
    public void UpdatePositionRange_WhenCalledMultipleTimes_ShouldRaiseMultipleEvents()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var member = AppUserDataFactory.Create(teamId: teamId);
        var team = TeamDataFactory.Create(id: teamId, members: [member]);
        
        var range1 = TeamPositionRange.Create(1, 10);
        var range2 = TeamPositionRange.Create(5, 15);
        
        var token1 = TeamValidators.PositionRangeUpdate.Validate(team, range1).Value!;
        var token2 = TeamValidators.PositionRangeUpdate.Validate(team, range2).Value!;

        // Act
        team.UpdatePositionRange(token1);
        team.UpdatePositionRange(token2);

        // Assert
        var domainEvents = team.GetDomainEvents();
        var positionEvents = domainEvents.OfType<TeamPositionRangeUpdatedDomainEvent>().ToList();
        positionEvents.Count.ShouldBe(2);
        
        positionEvents[0].Min.ShouldBe(1);
        positionEvents[0].Max.ShouldBe(10);
        
        positionEvents[1].Min.ShouldBe(5);
        positionEvents[1].Max.ShouldBe(15);
    }


    //------------------------//   


}//Cls
