using ID.Domain.Entities.AppUsers.ValueObjects;
using ID.Domain.Entities.Teams.Validators;
using ID.Tests.Data.Factories;
using Shouldly;

namespace ID.Domain.Tests.Entities.Teams.Validators;

public class MemberPositionUpdateValidatorTests
{
    [Fact]
    public void Validate_WhenMemberNotOnTeam_ShouldReturnFailure()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var teamMember = AppUserDataFactory.Create(teamId: teamId);
        var team = TeamDataFactory.Create(id: teamId, members: [teamMember]);
        var nonTeamMember = AppUserDataFactory.Create();
        var position = TeamPosition.Create(5);

        // Act
        var result = TeamValidators.MemberPositionUpdate.Validate(team, nonTeamMember, position);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Value.ShouldBeNull();
    }

    //------------------------// 

    [Fact]
    public void Validate_WhenUpdatingLeaderPosition_ShouldReturnFailure()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var leader = AppUserDataFactory.Create(teamId: teamId);
        var team = TeamDataFactory.Create(id: teamId, leader: leader);
        var position = TeamPosition.Create(5);

        // Act
        var result = TeamValidators.MemberPositionUpdate.Validate(team, leader, position);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Value.ShouldBeNull();
    }

    //------------------------// 

    [Fact]
    public void Validate_WhenPositionWithinRange_ShouldReturnSuccessWithSamePosition()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var member = AppUserDataFactory.Create(teamId: teamId);
        var team = TeamDataFactory.Create(id: teamId, minPosition: 1, maxPosition: 10, members: [member]);
        var position = TeamPosition.Create(5); // Within range

        // Act
        var result = TeamValidators.MemberPositionUpdate.Validate(team, member, position);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Team.ShouldBe(team);
        result.Value.Member.ShouldBe(member);
        result.Value.ClampedPosition.Value.ShouldBe(5); // Same as requested
    }

    //------------------------// 

    [Fact]
    public void Validate_WhenPositionBelowMinimum_ShouldReturnSuccessWithClampedPosition()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var member = AppUserDataFactory.Create(teamId: teamId);
        var team = TeamDataFactory.Create(id: teamId, minPosition: 5, maxPosition: 10, members: [member]);
        var position = TeamPosition.Create(2); // Below minimum

        // Act
        var result = TeamValidators.MemberPositionUpdate.Validate(team, member, position);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Team.ShouldBe(team);
        result.Value.Member.ShouldBe(member);
        result.Value.ClampedPosition.Value.ShouldBe(5); // Clamped to minimum
    }

    //------------------------// 

    [Fact]
    public void Validate_WhenPositionAboveMaximum_ShouldReturnSuccessWithClampedPosition()
    {
        // Arrange
        var member = AppUserDataFactory.Create();
        var team = TeamDataFactory.Create(minPosition: 1, maxPosition: 8, members: [member]);
        var position = TeamPosition.Create(15); // Above maximum

        // Act
        var result = TeamValidators.MemberPositionUpdate.Validate(team, member, position);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Team.ShouldBe(team);
        result.Value.Member.ShouldBe(member);
        result.Value.ClampedPosition.Value.ShouldBe(8); // Clamped to maximum
    }

    //------------------------// 

    [Theory]
    [InlineData(1, 10, 0, 1)]   // Below min, clamps to min
    [InlineData(1, 10, 5, 5)]   // Within range, stays same
    [InlineData(1, 10, 15, 10)] // Above max, clamps to max
    [InlineData(3, 7, 2, 3)]    // Below min, clamps to min
    [InlineData(3, 7, 8, 7)]    // Above max, clamps to max
    public void Validate_WithDifferentPositions_ShouldClampCorrectly(int minPos, int maxPos, int requestedPos, int expectedPos)
    {
        // Arrange
        var member = AppUserDataFactory.Create();
        var team = TeamDataFactory.Create(minPosition: minPos, maxPosition: maxPos, members: [member]);
        var position = TeamPosition.Create(requestedPos);

        // Act
        var result = TeamValidators.MemberPositionUpdate.Validate(team, member, position);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.ClampedPosition.Value.ShouldBe(expectedPos);
    }

    //------------------------// 

    [Fact]
    public void Validate_WithNonLeaderMember_ShouldReturnSuccess()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var leader = AppUserDataFactory.Create(teamId: teamId);
        var member = AppUserDataFactory.Create(teamId: teamId);
        var team = TeamDataFactory.Create(id: teamId, leader: leader, members: [member]);


        var position = TeamPosition.Create(5);

        // Act
        var result = TeamValidators.MemberPositionUpdate.Validate(team, member, position);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Member.ShouldBe(member);
        result.Value.Member.Id.ShouldNotBe(team.LeaderId!.Value); // Verify not leader
    }

    //------------------------// 

    [Fact]
    public void Token_ShouldStoreAllPropertiesCorrectly()
    {
        // Arrange
        var member = AppUserDataFactory.Create();
        var team = TeamDataFactory.Create(minPosition: 1, maxPosition: 10, members: [member]);
        var position = TeamPosition.Create(15); // Will be clamped to 10

        // Act
        var result = TeamValidators.MemberPositionUpdate.Validate(team, member, position);

        // Assert
        result.Succeeded.ShouldBeTrue();
        var token = result.Value!;
        token.Team.ShouldBe(team);
        token.Member.ShouldBe(member);
        token.ClampedPosition.Value.ShouldBe(10);
    }

    //------------------------// 

    [Fact]
    public void Token_ShouldImplementIValidationToken()
    {
        // Arrange
        var member = AppUserDataFactory.Create();
        var team = TeamDataFactory.Create(members: [member]);
        var position = TeamPosition.Create(5);

        // Act
        var result = TeamValidators.MemberPositionUpdate.Validate(team, member, position);

        // Assert
        result.Succeeded.ShouldBeTrue();
        var token = result.Value!;
        token.ShouldBeAssignableTo<IValidationToken>();
    }

    //------------------------// 

}//Cls

