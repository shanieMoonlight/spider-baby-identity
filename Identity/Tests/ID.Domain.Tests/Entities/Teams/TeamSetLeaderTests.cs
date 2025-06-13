using ID.Domain.Entities.Teams.Events;
using ID.Domain.Entities.Teams.Validators;
using ID.Tests.Data.Factories;
using Shouldly;

namespace ID.Domain.Tests.Entities.Teams;

public class TeamSetLeaderTests
{
    //------------------------------------//

    [Fact]
    public void SetLeader_WithValidToken_ShouldSetNewLeader()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var newLeader = AppUserDataFactory.Create(team:team);
        var validationResult = TeamValidators.LeaderUpdate.Validate(team, newLeader);
        
        validationResult.Succeeded.ShouldBeTrue();
        var token = validationResult.Value!;

        // Act
        var result = team.SetLeader(token);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldBe(team);
        team.LeaderId.ShouldBe(newLeader.Id);
        team.Leader.ShouldBe(newLeader);
    }

    //------------------------------------//

    [Fact]
    public void SetLeader_WithValidToken_ShouldSetLeaderPositionToMaximum()
    {
        // Arrange
        var team = TeamDataFactory.Create(minPosition: 1, maxPosition: 10);
        var newLeader = AppUserDataFactory.Create(team: team, teamPosition: 5);
        var validationResult = TeamValidators.LeaderUpdate.Validate(team, newLeader);
        var token = validationResult.Value!;

        // Act
        team.SetLeader(token);

        // Assert
        newLeader.TeamPosition.ShouldBe(team.MaxPosition);
    }

    //------------------------------------//

    [Fact]
    public void SetLeader_WithValidToken_ShouldRaiseDomainEvent()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var newLeader = AppUserDataFactory.Create(team: team);
        var validationResult = TeamValidators.LeaderUpdate.Validate(team, newLeader);
        var token = validationResult.Value!;

        var oldLeaderId = team.LeaderId; // Should be null initially

        // Act
        team.SetLeader(token);

        // Assert
        var domainEvents = team.GetDomainEvents();
        domainEvents.ShouldContain(e => e is TeamLeaderUpdatedDomainEvent);
        
        var leaderEvent = domainEvents.OfType<TeamLeaderUpdatedDomainEvent>().FirstOrDefault();
        leaderEvent.ShouldNotBeNull();
        leaderEvent.TeamId.ShouldBe(team.Id);
        leaderEvent.Team.ShouldBe(team);
        leaderEvent.NewLeaderId.ShouldBe(newLeader.Id);
        leaderEvent.OldLeaderId.ShouldBe(oldLeaderId);
    }

    //------------------------------------//

    [Fact]
    public void SetLeader_WhenChangingFromExistingLeader_ShouldUpdateCorrectly()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var oldLeader = AppUserDataFactory.Create(teamId: teamId);
        var team = TeamDataFactory.Create(id: teamId, leader: oldLeader);
        
        var newLeader = AppUserDataFactory.Create(teamId: teamId);
        var validationResult = TeamValidators.LeaderUpdate.Validate(team, newLeader);
        var token = validationResult.Value!;

        var oldLeaderId = team.LeaderId;

        // Act
        var result = team.SetLeader(token);

        // Assert
        result.Succeeded.ShouldBeTrue();
        team.LeaderId.ShouldBe(newLeader.Id);
        team.Leader.ShouldBe(newLeader);
        team.LeaderId.ShouldNotBe(oldLeaderId);
        
        // Verify domain event contains correct old/new leader IDs
        var domainEvents = team.GetDomainEvents();
        var leaderEvent = domainEvents.OfType<TeamLeaderUpdatedDomainEvent>().FirstOrDefault();
        leaderEvent.ShouldNotBeNull();
        leaderEvent.OldLeaderId.ShouldBe(oldLeaderId);
        leaderEvent.NewLeaderId.ShouldBe(newLeader.Id);
    }

    //------------------------------------//

    [Fact]
    public void SetLeader_ShouldReturnSuccessResult()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var newLeader = AppUserDataFactory.Create(team: team);
        var validationResult = TeamValidators.LeaderUpdate.Validate(team, newLeader);
        var token = validationResult.Value!;

        // Act
        var result = team.SetLeader(token);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldBeSameAs(team);
        result.Info.ShouldBeNullOrEmpty();
    }

    //------------------------------------//

    [Fact]
    public void SetLeader_WhenTeamHadNoLeader_ShouldSetFirstLeader()
    {
        // Arrange
        var team = TeamDataFactory.Create(); // No leader initially
        var newLeader = AppUserDataFactory.Create(team: team);
        var validationResult = TeamValidators.LeaderUpdate.Validate(team, newLeader);
        var token = validationResult.Value!;

        team.LeaderId.ShouldBeNull(); // Verify no leader initially

        // Act
        team.SetLeader(token);

        // Assert
        team.LeaderId.ShouldBe(newLeader.Id);
        team.Leader.ShouldBe(newLeader);
        
        // Verify domain event shows null as old leader
        var domainEvents = team.GetDomainEvents();
        var leaderEvent = domainEvents.OfType<TeamLeaderUpdatedDomainEvent>().FirstOrDefault();
        leaderEvent.ShouldNotBeNull();
        leaderEvent.OldLeaderId.ShouldBeNull();
        leaderEvent.NewLeaderId.ShouldBe(newLeader.Id);
    }

    //------------------------------------//

    [Theory]
    [InlineData(1, 5)]   // Min 1, Max 5
    [InlineData(3, 10)]  // Min 3, Max 10
    [InlineData(0, 20)]  // Min 0, Max 20
    public void SetLeader_WithDifferentPositionRanges_ShouldSetLeaderToMaxPosition(int minPosition, int maxPosition)
    {
        // Arrange
        var team = TeamDataFactory.Create(minPosition: minPosition, maxPosition: maxPosition);
        var newLeader = AppUserDataFactory.Create(team: team, teamPosition: minPosition); // Start at minimum
        var validationResult = TeamValidators.LeaderUpdate.Validate(team, newLeader);
        var token = validationResult.Value!;

        // Act
        team.SetLeader(token);

        // Assert
        newLeader.TeamPosition.ShouldBe(maxPosition);
        team.Leader.ShouldBe(newLeader);
    }

    //------------------------------------//

    [Fact]
    public void SetLeader_WhenNewLeaderHasLowerPosition_ShouldPromoteToMaxPosition()
    {
        // Arrange
        var team = TeamDataFactory.Create(minPosition: 1, maxPosition: 10);
        var newLeader = AppUserDataFactory.Create(team: team, teamPosition: 3); // Lower position
        var validationResult = TeamValidators.LeaderUpdate.Validate(team, newLeader);
        var token = validationResult.Value!;

        // Act
        team.SetLeader(token);

        // Assert
        newLeader.TeamPosition.ShouldBe(team.MaxPosition);
        team.LeaderId.ShouldBe(newLeader.Id);
    }

    //------------------------------------//

    [Fact]
    public void SetLeader_WhenNewLeaderAlreadyAtMaxPosition_ShouldMaintainPosition()
    {
        // Arrange
        var team = TeamDataFactory.Create(minPosition: 1, maxPosition: 10);
        var newLeader = AppUserDataFactory.Create(team: team, teamPosition: 10); // Already at max
        var validationResult = TeamValidators.LeaderUpdate.Validate(team, newLeader);
        var token = validationResult.Value!;

        // Act
        team.SetLeader(token);

        // Assert
        newLeader.TeamPosition.ShouldBe(team.MaxPosition);
        team.LeaderId.ShouldBe(newLeader.Id);
    }

    //------------------------------------//

    [Fact]
    public void SetLeader_WithMultipleLeaderChanges_ShouldTrackCorrectly()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var firstLeader =  AppUserDataFactory.Create(teamId: teamId);
        var secondLeader = AppUserDataFactory.Create(teamId: teamId);
        var thirdLeader =  AppUserDataFactory.Create(teamId: teamId);
        
        var team = TeamDataFactory.Create(id: teamId, members: [firstLeader, secondLeader, thirdLeader]);

        // Act 1: Set first leader
        var validation1 = TeamValidators.LeaderUpdate.Validate(team, firstLeader);
        team.SetLeader(validation1.Value!);
        var firstLeaderId = team.LeaderId;

        // Act 2: Change to second leader
        var validation2 = TeamValidators.LeaderUpdate.Validate(team, secondLeader);
        team.SetLeader(validation2.Value!);
        var secondLeaderId = team.LeaderId;

        // Act 3: Change to third leader
        var validation3 = TeamValidators.LeaderUpdate.Validate(team, thirdLeader);
        team.SetLeader(validation3.Value!);

        // Assert
        team.LeaderId.ShouldBe(thirdLeader.Id);
        team.Leader.ShouldBe(thirdLeader);
        
        // Verify progression of leadership
        firstLeaderId.ShouldBe(firstLeader.Id);
        secondLeaderId.ShouldBe(secondLeader.Id);
        team.LeaderId.ShouldBe(thirdLeader.Id);
        
        // Verify multiple domain events were raised
        var domainEvents = team.GetDomainEvents();
        var leaderEvents = domainEvents.OfType<TeamLeaderUpdatedDomainEvent>().ToList();
        leaderEvents.Count.ShouldBe(3);
    }

    //------------------------------------//

    [Fact]
    public void SetLeader_ShouldMaintainTeamMemberCollection()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var member1 = AppUserDataFactory.Create(teamId: teamId);
        var member2 = AppUserDataFactory.Create(teamId: teamId);
        var newLeader = AppUserDataFactory.Create(teamId: teamId);
        
        var team = TeamDataFactory.Create(id: teamId, members: [member1, member2, newLeader]);
        var originalMemberCount = team.Members.Count;

        var validationResult = TeamValidators.LeaderUpdate.Validate(team, newLeader);
        var token = validationResult.Value!;

        // Act
        team.SetLeader(token);

        // Assert
        team.Members.Count.ShouldBe(originalMemberCount);
        team.Members.ShouldContain(member1);
        team.Members.ShouldContain(member2);
        team.Members.ShouldContain(newLeader);
        team.Leader.ShouldBe(newLeader);
    }

    //------------------------------------//

}//Cls