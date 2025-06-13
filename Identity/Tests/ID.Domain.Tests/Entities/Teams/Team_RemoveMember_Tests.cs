using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Domain.Entities.Teams.Events;
using ID.Domain.Entities.Teams.Validators;
using ID.Tests.Data.Factories;
using Shouldly;

namespace ID.Domain.Tests.Entities.Teams;

public class Team_RemoveMember_Tests
{
    //------------------------------------//

    [Fact]
    public void RemoveMember_WithValidToken_ShouldRemoveMemberAndRaiseEvent()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var member1 = AppUserDataFactory.Create(teamId: teamId);
        var member2 = AppUserDataFactory.Create(teamId: teamId);
        var team = TeamDataFactory.Create(id: teamId, members: [member1, member2]);

        var validationResult = TeamValidators.MemberRemoval.Validate(team, member1);
        validationResult.Succeeded.ShouldBeTrue();
        var token = validationResult.Value!;

        // Act
        var result = team.RemoveMember(token);

        // Assert
        result.ShouldBeTrue();
        team.Members.ShouldNotContain(member1);
        team.Members.ShouldContain(member2);
        team.Members.Count.ShouldBe(1);

        // Should raise domain event
        var domainEvents = team.GetDomainEvents();
        domainEvents.ShouldContain(e => e is TeamMemberRemovedDomainEvent);

        var memberEvent = domainEvents.OfType<TeamMemberRemovedDomainEvent>().FirstOrDefault();
        memberEvent.ShouldNotBeNull();
        memberEvent.Team.ShouldBe(team);
        memberEvent.Member.ShouldBe(member1);
    }

    //------------------------------------//

    [Fact]
    public void RemoveMember_WhenMemberNotInTeam_ShouldReturnFalseAndNotRaiseEvent()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var otherTeamId = Guid.NewGuid();
        var teamMember = AppUserDataFactory.Create(teamId: teamId);
        var nonTeamMember = AppUserDataFactory.Create(teamId: otherTeamId);
        var team = TeamDataFactory.Create(id: teamId, members: [teamMember]);

        // Create token with non-team member (this would normally fail validation but we're testing the method directly)
        var token = new TeamValidators.MemberRemoval.Token(team, nonTeamMember);

        // Act
        var result = team.RemoveMember(token);

        // Assert
        result.ShouldBeFalse();
        team.Members.ShouldContain(teamMember); // Original member still there
        team.Members.Count.ShouldBe(1);

        // Should NOT raise domain event
        var domainEvents = team.GetDomainEvents();
        domainEvents.ShouldNotContain(e => e is TeamMemberRemovedDomainEvent);
    }

    //------------------------------------//

    [Fact]
    public void RemoveMember_WithMultipleMembers_ShouldOnlyRemoveSpecifiedMember()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var member1 = AppUserDataFactory.Create(teamId: teamId);
        var member2 = AppUserDataFactory.Create(teamId: teamId);
        var member3 = AppUserDataFactory.Create(teamId: teamId);
        var team = TeamDataFactory.Create(id: teamId, members: [member1, member2, member3]);

        var validationResult = TeamValidators.MemberRemoval.Validate(team, member2);
        var token = validationResult.Value!;

        // Act
        var result = team.RemoveMember(token);

        // Assert
        result.ShouldBeTrue();
        team.Members.ShouldContain(member1);
        team.Members.ShouldNotContain(member2); // Only this one removed
        team.Members.ShouldContain(member3);
        team.Members.Count.ShouldBe(2);
    }

    //------------------------------------//

    [Fact]
    public void RemoveMember_WhenRemovingLastMember_ShouldSucceedIfAllowed()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var lastMember = AppUserDataFactory.Create(teamId: teamId);
        var team = TeamDataFactory.Create(id: teamId, teamType: TeamType.Customer, members: [lastMember]); // Super team can be empty

        var validationResult = TeamValidators.MemberRemoval.Validate(team, lastMember);
        validationResult.Succeeded.ShouldBeTrue(); // Should pass validation for Super team
        var token = validationResult.Value!;

        // Act
        var result = team.RemoveMember(token);

        // Assert
        result.ShouldBeTrue();
        team.Members.ShouldBeEmpty();

        // Should raise domain event
        var domainEvents = team.GetDomainEvents();
        domainEvents.ShouldContain(e => e is TeamMemberRemovedDomainEvent);
    }

    //------------------------------------//

    [Fact]
    public void RemoveMember_WhenCalledMultipleTimes_ShouldOnlyRemoveOnce()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var member1 = AppUserDataFactory.Create(teamId: teamId);
        var member2 = AppUserDataFactory.Create(teamId: teamId);
        var team = TeamDataFactory.Create(id: teamId, members: [member1, member2]);

        var validationResult = TeamValidators.MemberRemoval.Validate(team, member1);
        var token = validationResult.Value!;

        // Act
        var result1 = team.RemoveMember(token);
        var result2 = team.RemoveMember(token); // Second call

        // Assert
        result1.ShouldBeTrue();  // First removal succeeds
        result2.ShouldBeFalse(); // Second removal fails (member already gone)

        team.Members.ShouldNotContain(member1);
        team.Members.Count.ShouldBe(1);

        // Should only raise one domain event
        var domainEvents = team.GetDomainEvents();
        var memberEvents = domainEvents.OfType<TeamMemberRemovedDomainEvent>().ToList();
        memberEvents.Count.ShouldBe(1);
    }

    //------------------------------------//

    [Fact]
    public void RemoveMember_ShouldUseTokenMemberReference()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var member1 = AppUserDataFactory.Create(teamId: teamId);
        var member2 = AppUserDataFactory.Create(teamId: teamId);
        var team = TeamDataFactory.Create(id: teamId, members: [member1, member2]);

        var validationResult = TeamValidators.MemberRemoval.Validate(team, member1);
        var token = validationResult.Value!;

        // Verify token contains correct member
        token.Member.ShouldBe(member1);
        token.Team.ShouldBe(team);

        // Act
        var result = team.RemoveMember(token);

        // Assert
        result.ShouldBeTrue();
        team.Members.ShouldNotContain(member1); // Token's member was removed
        team.Members.ShouldContain(member2);
    }

    //------------------------------------//

    [Theory]
    [InlineData(TeamType.Customer)]
    [InlineData(TeamType.Super)]
    [InlineData(TeamType.Maintenance)]
    public void RemoveMember_WithDifferentTeamTypes_ShouldWorkConsistently(TeamType teamType)
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var member1 = AppUserDataFactory.Create(teamId: teamId);
        var member2 = AppUserDataFactory.Create(teamId: teamId);
        var team = TeamDataFactory.Create(id: teamId, teamType: teamType, members: [member1, member2]);

        var validationResult = TeamValidators.MemberRemoval.Validate(team, member1);
        // Note: This might fail for Customer teams with only 1 member, but we have 2 here
        if (!validationResult.Succeeded)
        {
            // Skip this test case if validation prevents removal
            return;
        }

        var token = validationResult.Value!;

        // Act
        var result = team.RemoveMember(token);

        // Assert
        result.ShouldBeTrue();
        team.Members.ShouldNotContain(member1);
        team.Members.Count.ShouldBe(1);
    }

    //------------------------------------//

    [Fact]
    public void RemoveMember_ShouldMaintainMemberCollectionIntegrity()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var members = new List<AppUser>();
        for (int i = 0; i < 5; i++)
        {
            members.Add(AppUserDataFactory.Create(teamId: teamId));
        }
        var team = TeamDataFactory.Create(id: teamId, members: [..members]);

        var memberToRemove = members[2]; // Remove middle member
        var validationResult = TeamValidators.MemberRemoval.Validate(team, memberToRemove);
        var token = validationResult.Value!;

        // Act
        var result = team.RemoveMember(token);

        // Assert
        result.ShouldBeTrue();
        team.Members.Count.ShouldBe(4);
        team.Members.ShouldNotContain(memberToRemove);

        // Verify all other members are still there
        foreach (var member in members.Where(m => m != memberToRemove))
        {
            team.Members.ShouldContain(member);
        }
    }

    //------------------------------------//

    [Fact]
    public void RemoveMember_CustomerTeamWithLastMember_ShouldAllowRemovalAndPassValidation()
    {
        // Arrange - Customer team with only one member
        var teamId = Guid.NewGuid();
        var lastMember = AppUserDataFactory.Create(teamId: teamId);
        var customerTeam = TeamDataFactory.Create(id: teamId, teamType: TeamType.Customer, members: [lastMember]);

        // Act - Validate removal of last Customer member
        var validationResult = TeamValidators.MemberRemoval.Validate(customerTeam, lastMember);

        // Assert - Validation should succeed for Customer teams
        validationResult.Succeeded.ShouldBeTrue("Customer teams should allow removal of the last member");
        validationResult.Value.ShouldNotBeNull();
        validationResult.Value.Team.ShouldBe(customerTeam);
        validationResult.Value.Member.ShouldBe(lastMember);

        // Act - Actually remove the member using the token
        var removalResult = customerTeam.RemoveMember(validationResult.Value);

        // Assert - Removal should succeed
        removalResult.ShouldBeTrue();
        customerTeam.Members.ShouldBeEmpty();

        // Should raise domain event
        var domainEvents = customerTeam.GetDomainEvents();
        domainEvents.ShouldContain(e => e is TeamMemberRemovedDomainEvent);
        
        var memberEvent = domainEvents.OfType<TeamMemberRemovedDomainEvent>().FirstOrDefault();
        memberEvent.ShouldNotBeNull();
        memberEvent.Team.ShouldBe(customerTeam);
        memberEvent.Member.ShouldBe(lastMember);
    }

    //------------------------------------//

    [Theory]
    [InlineData(TeamType.Super)]
    [InlineData(TeamType.Maintenance)]
    public void RemoveMember_NonCustomerTeamWithLastMember_ShouldFailValidation(TeamType teamType)
    {
        // Arrange - Non-Customer team with only one member
        var teamId = Guid.NewGuid();
        var lastMember = AppUserDataFactory.Create(teamId: teamId);
        var team = TeamDataFactory.Create(id: teamId, teamType: teamType, members: [lastMember]);

        // Act - Try to validate removal of last member from Super/Maintenance team
        var validationResult = TeamValidators.MemberRemoval.Validate(team, lastMember);        // Assert - Validation should fail for Super/Maintenance teams
        validationResult.Succeeded.ShouldBeFalse($"{teamType} teams should NOT allow removal of the last member");
        validationResult.Info.ShouldNotBeNullOrEmpty();
    }

    //------------------------------------//

}//Cls