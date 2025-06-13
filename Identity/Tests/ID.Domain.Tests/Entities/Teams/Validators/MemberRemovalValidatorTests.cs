using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Domain.Entities.Teams.Validators;
using ID.Tests.Data.Factories;
using Shouldly;

namespace ID.Domain.Tests.Entities.Teams.Validators;

public class MemberRemovalValidatorTests
{

    [Fact]
    public void Validate_WithValidMemberRemoval_ShouldSucceed()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var member1 = AppUserDataFactory.Create(teamId: teamId);
        var member2 = AppUserDataFactory.Create(teamId: teamId);
        var team = TeamDataFactory.Create(id: teamId, teamType: TeamType.Customer, members: [member1, member2]);

        // Act
        var result = TeamValidators.MemberRemoval.Validate(team, member1);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Team.ShouldBe(team);
        result.Value.Member.ShouldBe(member1);
    }

    //------------------------------------//

    [Fact]
    public void Validate_WhenMemberNotOnTeam_ShouldFail()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var otherTeamId = Guid.NewGuid();
        var teamMember = AppUserDataFactory.Create(teamId: teamId);
        var nonTeamMember = AppUserDataFactory.Create(teamId: otherTeamId); // Different team
        var team = TeamDataFactory.Create(id: teamId, members: [teamMember]);

        // Act
        var result = TeamValidators.MemberRemoval.Validate(team, nonTeamMember);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Value.ShouldBeNull();
    }

    //------------------------------------//

    [Theory]
    [InlineData(TeamType.Super)]
    [InlineData(TeamType.Maintenance)]
    public void Validate_WhenRemovingLastMemberFromSystemTeam_ShouldFail(TeamType teamType)
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var lastMember = AppUserDataFactory.Create(teamId: teamId);
        var team = TeamDataFactory.Create(id: teamId, teamType: teamType, members: [lastMember]);

        // Act
        var result = TeamValidators.MemberRemoval.Validate(team, lastMember);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Value.ShouldBeNull();
    }

    //------------------------------------//

    [Fact]
    public void Validate_WhenRemovingLastMemberFromCustomerTeam_ShouldSucceed()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var lastMember = AppUserDataFactory.Create(teamId: teamId);
        var team = TeamDataFactory.Create(id: teamId, teamType: TeamType.Customer, members: [lastMember]);

        // Act
        var result = TeamValidators.MemberRemoval.Validate(team, lastMember);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Team.ShouldBe(team);
        result.Value.Member.ShouldBe(lastMember);
    }

    //------------------------------------//

    [Fact]
    public void Validate_WhenRemovingTeamLeader_ShouldFail()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var leader = AppUserDataFactory.Create(teamId: teamId);
        var member = AppUserDataFactory.Create(teamId: teamId);
        var team = TeamDataFactory.Create(id: teamId, leader: leader, members: [leader, member]);

        // Act
        var result = TeamValidators.MemberRemoval.Validate(team, leader);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldContain("team leader"); // Assuming this is in your error message
        result.Value.ShouldBeNull();
    }

    //------------------------------------//

    [Fact]
    public void Validate_WhenRemovingNonLeaderMember_ShouldSucceed()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var leader = AppUserDataFactory.Create(teamId: teamId);
        var regularMember = AppUserDataFactory.Create(teamId: teamId);
        var team = TeamDataFactory.Create(id: teamId, leader: leader, members: [leader, regularMember]);

        // Act
        var result = TeamValidators.MemberRemoval.Validate(team, regularMember);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Team.ShouldBe(team);
        result.Value.Member.ShouldBe(regularMember);
    }

    //------------------------------------//

    [Theory]
    [InlineData(TeamType.Customer, 3)]
    [InlineData(TeamType.Super, 5)]
    [InlineData(TeamType.Maintenance, 2)]
    public void Validate_WithMultipleMembersInDifferentTeamTypes_ShouldSucceed(TeamType teamType, int memberCount)
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var members = new List<AppUser>();
        for (int i = 0; i < memberCount; i++)
        {
            members.Add(AppUserDataFactory.Create(teamId: teamId));
        }
        var team = TeamDataFactory.Create(id: teamId, teamType: teamType, members: [..members]);
        var memberToRemove = members.First();

        // Act
        var result = TeamValidators.MemberRemoval.Validate(team, memberToRemove);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Member.ShouldBe(memberToRemove);
    }

    //------------------------------------//

    [Fact]
    public void Validate_TokenShouldImplementIValidationToken()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var member = AppUserDataFactory.Create(teamId: teamId);
        var member2 = AppUserDataFactory.Create(teamId: teamId);
        var team = TeamDataFactory.Create(id: teamId, members: [member, member2]);

        // Act
        var result = TeamValidators.MemberRemoval.Validate(team, member);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldBeAssignableTo<IValidationToken>();
    }

    //------------------------------------//

    [Fact]
    public void Validate_WhenMemberBelongsToDifferentTeamButSameId_ShouldFail()
    {
        // Arrange - Edge case: member has same ID as team but belongs to different team
        var teamId = Guid.NewGuid();
        var otherTeamId = Guid.NewGuid();
        var teamMember = AppUserDataFactory.Create(teamId: teamId);
        var memberFromOtherTeam = AppUserDataFactory.Create(teamId: otherTeamId);
        var team = TeamDataFactory.Create(id: teamId, members: [teamMember]);

        // Act
        var result = TeamValidators.MemberRemoval.Validate(team, memberFromOtherTeam);

        // Assert
        result.Succeeded.ShouldBeFalse();
    }

    //------------------------------------//

    [Fact]
    public void Validate_WhenTeamHasNoLeader_ShouldSucceedForRegularMembers()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var member1 = AppUserDataFactory.Create(teamId: teamId);
        var member2 = AppUserDataFactory.Create(teamId: teamId);
        var team = TeamDataFactory.Create(id: teamId, leader: null, members: [member1, member2]); // No leader

        // Act
        var result = TeamValidators.MemberRemoval.Validate(team, member1);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Member.ShouldBe(member1);
    }

    //------------------------------------//

    [Fact]
    public void Validate_MultipleValidationRulesCanFail_ShouldReturnFirstFailure()
    {
        // Arrange - Member not on team AND it's the last member of a system team
        var teamId = Guid.NewGuid();
        var otherTeamId = Guid.NewGuid();
        var memberFromOtherTeam = AppUserDataFactory.Create(teamId: otherTeamId);
        var team = TeamDataFactory.Create(id: teamId, teamType: TeamType.Super, members: []); // Empty system team

        // Act
        var result = TeamValidators.MemberRemoval.Validate(team, memberFromOtherTeam);

        // Assert
        result.Succeeded.ShouldBeFalse();
        // Should fail on first rule (member not on team) before checking other rules
    }

    //------------------------------------//

    [Theory]
    [InlineData(TeamType.Super)]
    [InlineData(TeamType.Maintenance)]
    public void Validate_WhenNonCustomerTeamLeaderIsLastMember_ShouldStillFailLeaderCheck(TeamType teamType)
    {
        // Arrange - Leader is last member, should fail on leader rule regardless of team type
        var teamId = Guid.NewGuid();
        var leader = AppUserDataFactory.Create(teamId: teamId);
        var team = TeamDataFactory.Create(id: teamId, teamType: teamType, leader: leader, members: [leader]);

        // Act
        var result = TeamValidators.MemberRemoval.Validate(team, leader);

        // Assert
        result.Succeeded.ShouldBeFalse();
    }

}//Cls