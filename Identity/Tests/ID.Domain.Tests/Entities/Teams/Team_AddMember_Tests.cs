using ID.Domain.Entities.Teams.Events;
using ID.Domain.Entities.Teams.Validators;
using ID.Tests.Data.Factories;
using Shouldly;

namespace ID.Domain.Tests.Entities.Teams;

public class TeamAddMemberTests
{
    //------------------------------------//

    [Fact]
    public void AddMember_WithValidToken_ShouldAddMemberToTeam()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var member = AppUserDataFactory.Create();
        var validationResult = TeamValidators.MemberAddition.Validate(team, member);
        
        validationResult.Succeeded.ShouldBeTrue();
        var token = validationResult.Value!;

        // Act
        var result = team.AddMember(token);

        // Assert
        result.ShouldBe(team);
        team.Members.ShouldContain(member);
        member.TeamId.ShouldBe(team.Id);
    }

    //------------------------------------//

    [Fact]
    public void AddMember_WithValidToken_ShouldSetMemberTeamPosition()
    {
        // Arrange
        var team = TeamDataFactory.Create(minPosition: 1, maxPosition: 10);
        var member = AppUserDataFactory.Create();
        var validationResult = TeamValidators.MemberAddition.Validate(team, member);
        var token = validationResult.Value!;

        // Act
        team.AddMember(token);

        // Assert
        member.TeamPosition.ShouldBeGreaterThanOrEqualTo(team.MinPosition);
        member.TeamPosition.ShouldBeLessThanOrEqualTo(team.MaxPosition);
    }

    //------------------------------------//

    [Fact]
    public void AddMember_WhenTeamHasNoLeader_ShouldSetMemberAsLeader()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var member = AppUserDataFactory.Create();
        var validationResult = TeamValidators.MemberAddition.Validate(team, member);
        var token = validationResult.Value!;

        team.LeaderId.ShouldBeNull(); // Ensure no leader initially

        // Act
        team.AddMember(token);

        // Assert
        team.LeaderId.ShouldBe(member.Id);
        team.Leader.ShouldBe(member);
        member.TeamPosition.ShouldBe(team.MaxPosition);
    }

    //------------------------------------//

    [Fact]
    public void AddMember_WhenTeamHasLeader_ShouldNotChangeLeader()
    {
        // Arrange
        var existingLeader = AppUserDataFactory.Create();
        var team = TeamDataFactory.Create(leader: existingLeader);
        
        var newMember = AppUserDataFactory.Create();
        var validationResult = TeamValidators.MemberAddition.Validate(team, newMember);
        var token = validationResult.Value!;

        var originalLeaderId = team.LeaderId;

        // Act
        team.AddMember(token);

        // Assert
        team.LeaderId.ShouldBe(originalLeaderId);
        team.Leader.ShouldBe(existingLeader);
        team.Members.ShouldContain(newMember);
    }

    //------------------------------------//

    [Fact]
    public void AddMember_WithValidToken_ShouldRaiseDomainEvent()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var member = AppUserDataFactory.Create();
        var validationResult = TeamValidators.MemberAddition.Validate(team, member);
        var token = validationResult.Value!;

        // Act
        team.AddMember(token);

        // Assert
        var domainEvents = team.GetDomainEvents();
        domainEvents.ShouldContain(e => e is TeamMemberAddedDomainEvent); 
        
        var memberEvent = domainEvents.OfType<TeamMemberAddedDomainEvent>().FirstOrDefault();
        memberEvent.ShouldNotBeNull();
        memberEvent.Team.ShouldBe(team);
        memberEvent.User.ShouldBe(member);
    }

    //------------------------------------//

    [Fact]
    public void AddMember_WhenMemberPositionBelowMinimum_AndTeamAlreadyHasLeader_ShouldAdjustToMinimum()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var leader = AppUserDataFactory.Create(teamId:teamId); 
        var team = TeamDataFactory.Create(id: teamId, minPosition: 5, maxPosition: 15, leader:leader);
        var member = AppUserDataFactory.Create(teamPosition: 2); // Below minimum
        var validationResult = TeamValidators.MemberAddition.Validate(team, member);
        var token = validationResult.Value!;

        // Act
        team.AddMember(token);

        // Assert
        member.TeamPosition.ShouldBe(team.MinPosition);
    }

    //------------------------------------//

    [Fact]
    public void AddMember_WhenMemberPositionAboveMaximum_ShouldAdjustToMaximum()
    {
        // Arrange
        var team = TeamDataFactory.Create(minPosition: 5, maxPosition: 15);
        var member = AppUserDataFactory.Create(teamPosition: 20); // Above maximum
        var validationResult = TeamValidators.MemberAddition.Validate(team, member);
        var token = validationResult.Value!;

        // Act
        team.AddMember(token);

        // Assert
        member.TeamPosition.ShouldBe(team.MaxPosition);
    }

    //------------------------------------//

    [Fact]
    public void AddMember_WhenMemberPositionWithinRange_AndTeamAlreadyHasLeader_ShouldKeepOriginalPosition()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var leader = AppUserDataFactory.Create(teamId: teamId);
        var team = TeamDataFactory.Create(id: teamId, minPosition: 5, maxPosition: 15, leader: leader);
        var originalPosition = 10;
        var member = AppUserDataFactory.Create(teamPosition: originalPosition);
        var validationResult = TeamValidators.MemberAddition.Validate(team, member);
        var token = validationResult.Value!;

        // Act
        team.AddMember(token);

        // Assert
        member.TeamPosition.ShouldBe(originalPosition);
    }

    //------------------------------------//

    [Fact]
    public void AddMember_ShouldReturnSameTeamInstance()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var member = AppUserDataFactory.Create();
        var validationResult = TeamValidators.MemberAddition.Validate(team, member);
        var token = validationResult.Value!;

        // Act
        var result = team.AddMember(token);

        // Assert
        result.ShouldBeSameAs(team);
    }

    //------------------------------------//

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void AddMember_WithDifferentTeamCapacities_ShouldSucceedWhenUnderCapacity(int capacity)
    {
        // Arrange
        var team = TeamDataFactory.Create(capacity: capacity);
        var member = AppUserDataFactory.Create();
        var validationResult = TeamValidators.MemberAddition.Validate(team, member);
        var token = validationResult.Value!;

        team.Members.Count.ShouldBeLessThan(capacity);

        // Act
        var result = team.AddMember(token);

        // Assert
        result.ShouldBe(team);
        team.Members.ShouldContain(member);
    }

    //------------------------------------//

    [Fact]
    public void AddMember_WithMultipleMembers_ShouldMaintainMemberCollection()
    {
        // Arrange
        var existingMember1 = AppUserDataFactory.Create();
        var existingMember2 = AppUserDataFactory.Create();
        var team = TeamDataFactory.Create(members: [existingMember1, existingMember2]);
        
        var newMember = AppUserDataFactory.Create();
        var validationResult = TeamValidators.MemberAddition.Validate(team, newMember);
        var token = validationResult.Value!;

        // Act
        team.AddMember(token);

        // Assert
        team.Members.Count.ShouldBe(3);
        team.Members.ShouldContain(existingMember1);
        team.Members.ShouldContain(existingMember2);
        team.Members.ShouldContain(newMember);
    }

    //------------------------------------//

}//Cls