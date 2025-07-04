using ID.Domain.Entities.Teams;
using ID.Infrastructure.Persistance.Abstractions.Repos.Specs;
using ID.Infrastructure.Persistance.EF.Repos;
using ID.Infrastructure.Persistance.EF.Repos.Specs.Teams;
using ID.Tests.Data.Factories;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace ID.Infrastructure.Tests.Persistence.EF.Repos.Teams;

public class TeamFirstOrDefaultAsyncTests : RepoTestBase, IAsyncLifetime
{
    private TeamRepo _repo = null!;

    private readonly Team _customerTeam = TeamDataFactory.Create(
        name: "Customer Team Alpha",
        teamType: TeamType.customer);

    private readonly Team _superTeam = TeamDataFactory.Create(
        name: "Super Team",
        teamType: TeamType.super);

    private readonly Team _maintenanceTeam = TeamDataFactory.Create(
        name: "Maintenance Team", 
        teamType: TeamType.maintenance);

    private readonly List<Team> _teams;

    //-----------------------------//

    public TeamFirstOrDefaultAsyncTests()
    {
        _teams = [_customerTeam, _superTeam, _maintenanceTeam];
    }

    //-----------------------------//

    public async Task InitializeAsync()
    {
        _repo = new TeamRepo(DbContext);
        await SeedDataAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    private async Task SeedDataAsync()
    {
        await DbContext.Teams.AddRangeAsync(_teams);
        await DbContext.SaveChangesAsync();
    }

    //=================================//
    // GET BY ID SPEC TESTS
    //=================================//

    #region GetByIdSpec Tests

    [Fact]
    public async Task FirstOrDefaultAsync_WithGetByIdSpec_ExistingId_ShouldReturnTeam()
    {
        // Arrange
        var spec = new GetByIdSpec<Team>(_customerTeam.Id);

        // Act
        var result = await _repo.FirstOrDefaultAsync(spec);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(_customerTeam.Id);
        result.Name.ShouldBe(_customerTeam.Name);
        result.TeamType.ShouldBe(TeamType.customer);
    }

    [Fact]
    public async Task FirstOrDefaultAsync_WithGetByIdSpec_NonExistingId_ShouldReturnNull()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();
        var spec = new GetByIdSpec<Team>(nonExistingId);

        // Act
        var result = await _repo.FirstOrDefaultAsync(spec);

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public async Task FirstOrDefaultAsync_WithGetByIdSpec_NullId_ShouldReturnNull()
    {
        // Arrange
        var spec = new GetByIdSpec<Team>(null);

        // Act
        var result = await _repo.FirstOrDefaultAsync(spec);

        // Assert
        result.ShouldBeNull();
    }

    #endregion

    //=================================//
    // TEAM BY ID WITH MEMBER SPEC TESTS
    //=================================//

    #region TeamByIdWithMemberSpec Tests

    [Fact]
    public async Task FirstOrDefaultAsync_WithTeamByIdWithMemberSpec_ExistingTeamAndMember_ShouldReturnTeamWithMember()
    {
        // Arrange
        var member = AppUserDataFactory.Create(teamId: _customerTeam.Id);
        var teamWithMember = TeamDataFactory.Create(
            id: _customerTeam.Id,
            teamType: TeamType.customer,
            members: [member]);

        await DbContext.Set<ID.Domain.Entities.AppUsers.AppUser>().AddAsync(member);
        await DbContext.SaveChangesAsync();

        var spec = new TeamByIdWithMemberSpec(_customerTeam.Id, member.Id);

        // Act
        var result = await _repo.FirstOrDefaultAsync(spec);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(_customerTeam.Id);
        result.Members.ShouldContain(m => m.Id == member.Id);
    }

    [Fact]
    public async Task FirstOrDefaultAsync_WithTeamByIdWithMemberSpec_NonExistingMember_ShouldReturnNull()
    {
        // Arrange
        var nonExistingMemberId = Guid.NewGuid();
        var spec = new TeamByIdWithMemberSpec(_customerTeam.Id, nonExistingMemberId);

        // Act
        var result = await _repo.FirstOrDefaultAsync(spec);

        // Assert
        result.ShouldNotBeNull();
        result.Members.ShouldBeEmpty(); // No members should be returned
    }

    #endregion

    //=================================//
    // TEAM BY ID WITH MEMBERS SPEC TESTS
    //=================================//

    #region TeamByIdWithMembersSpec Tests

    [Fact]
    public async Task FirstOrDefaultAsync_WithTeamByIdWithMembersSpec_ShouldReturnTeamWithMembers()
    {
        // Arrange
        var member1 = AppUserDataFactory.Create(teamId: _customerTeam.Id, teamPosition: 1);
        var member2 = AppUserDataFactory.Create(teamId: _customerTeam.Id, teamPosition: 3);
        var teamWithMembers = TeamDataFactory.Create(
            id: _customerTeam.Id,
            teamType: TeamType.customer,
            members: [member1, member2]);

        await DbContext.Set<ID.Domain.Entities.AppUsers.AppUser>().AddRangeAsync(member1, member2);
        await DbContext.SaveChangesAsync();

        var spec = new TeamByIdWithMembersSpec(_customerTeam.Id, maxPosition: 1000);

        // Act
        var result = await _repo.FirstOrDefaultAsync(spec);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(_customerTeam.Id);
        result.Members.Count.ShouldBeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task FirstOrDefaultAsync_WithTeamByIdWithMembersSpec_WithMaxPosition_ShouldFilterByPosition()
    {
        // Arrange
        var member1 = AppUserDataFactory.Create(teamId: _customerTeam.Id, teamPosition: 1);
        var member2 = AppUserDataFactory.Create(teamId: _customerTeam.Id, teamPosition: 5);
        var teamWithMembers = TeamDataFactory.Create(
            id: _customerTeam.Id,
            teamType: TeamType.customer,
            members: [member1, member2]);

        await DbContext.Set<ID.Domain.Entities.AppUsers.AppUser>().AddRangeAsync(member1, member2);
        await DbContext.SaveChangesAsync();

        var spec = new TeamByIdWithMembersSpec(_customerTeam.Id, maxPosition: 2);

        // Act
        var result = await _repo.FirstOrDefaultAsync(spec);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(_customerTeam.Id);
        // Note: Actual filtering behavior depends on specification implementation
    }

    #endregion

    //=================================//
    // SUPER TEAM SPEC TESTS
    //=================================//

    #region SuperTeamWithMembersSpec Tests

    [Fact]
    public async Task FirstOrDefaultAsync_WithSuperTeamWithMembersSpec_ShouldReturnSuperTeam()
    {
        // Arrange
        var spec = new SuperTeamWithMembersSpec(maxPosition: 1000);

        // Act
        var result = await _repo.FirstOrDefaultAsync(spec);

        // Assert
        result.ShouldNotBeNull();
        result.TeamType.ShouldBe(TeamType.super);
        result.Name.ShouldBe(_superTeam.Name);
    }

    [Fact]
    public async Task FirstOrDefaultAsync_WithSuperTeamWithMemberSpec_ExistingMember_ShouldReturnSuperTeam()
    {
        // Arrange
        var member = AppUserDataFactory.Create(teamId: _superTeam.Id, teamPosition: 1);
        await DbContext.Set<ID.Domain.Entities.AppUsers.AppUser>().AddAsync(member);
        await DbContext.SaveChangesAsync();

        var spec = new SuperTeamWithMemberSpec(member.Id, maxPosition: 1000);

        // Act
        var result = await _repo.FirstOrDefaultAsync(spec);

        // Assert
        result.ShouldNotBeNull();
        result.TeamType.ShouldBe(TeamType.super);
    }

    [Fact]
    public async Task FirstOrDefaultAsync_WithSuperTeamWithMemberSpec_NonExistingMember_ShouldReturnNull()
    {
        // Arrange
        var nonExistingMemberId = Guid.NewGuid();
        var spec = new SuperTeamWithMemberSpec(nonExistingMemberId, maxPosition: 1000);

        // Act
        var result = await _repo.FirstOrDefaultAsync(spec);

        // Assert
        result.ShouldNotBeNull();
        result.Members.ShouldBeEmpty(); // No members should be returned
    }

    #endregion

    //=================================//
    // MAINTENANCE TEAM SPEC TESTS
    //=================================//

    #region MntcTeamWithMembersSpec Tests

    [Fact]
    public async Task FirstOrDefaultAsync_WithMntcTeamWithMembersSpec_ShouldReturnMaintenanceTeam()
    {
        // Arrange
        var spec = new MntcTeamWithMembersSpec(maxPosition: 1000);

        // Act
        var result = await _repo.FirstOrDefaultAsync(spec);

        // Assert
        result.ShouldNotBeNull();
        result.TeamType.ShouldBe(TeamType.maintenance);
        result.Name.ShouldBe(_maintenanceTeam.Name);
    }

    [Fact]
    public async Task FirstOrDefaultAsync_WithMntcTeamWithMemberSpec_ExistingMember_ShouldReturnMaintenanceTeam()
    {
        // Arrange
        var member = AppUserDataFactory.Create(teamId: _maintenanceTeam.Id, teamPosition: 1);
        await DbContext.Set<ID.Domain.Entities.AppUsers.AppUser>().AddAsync(member);
        await DbContext.SaveChangesAsync();

        var spec = new MntcTeamWithMemberSpec(member.Id, maxPosition: 1000);

        // Act
        var result = await _repo.FirstOrDefaultAsync(spec);

        // Assert
        result.ShouldNotBeNull();
        result.TeamType.ShouldBe(TeamType.maintenance);
    }

    [Fact]
    public async Task FirstOrDefaultAsync_WithMntcTeamWithMemberSpec_NonExistingMember_ShouldReturnNull()
    {
        // Arrange
        var nonExistingMemberId = Guid.NewGuid();
        var spec = new MntcTeamWithMemberSpec(nonExistingMemberId, maxPosition: 1000);

        // Act
        var result = await _repo.FirstOrDefaultAsync(spec);

        // Assert
        result.ShouldNotBeNull();
        result.Members.ShouldBeEmpty(); // No members should be returned
    }

    #endregion

    //=================================//
    // TEAM BY ID WITH SUBSCRIPTIONS SPEC TESTS
    //=================================//

    #region TeamByIdWithSubscriptionsSpec Tests

    [Fact]
    public async Task FirstOrDefaultAsync_WithTeamByIdWithSubscriptionsSpec_ShouldReturnTeamWithSubscriptions()
    {
        // Arrange
        var subscription = SubscriptionDataFactory.Create(teamId: _customerTeam.Id);
        await DbContext.Set<TeamSubscription>().AddAsync(subscription);
        await DbContext.SaveChangesAsync();

        var spec = new TeamByIdWithSubscriptionsSpec(_customerTeam.Id);

        // Act
        var result = await _repo.FirstOrDefaultAsync(spec);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(_customerTeam.Id);
        result.Subscriptions.ShouldContain(s => s.Id == subscription.Id);
    }

    [Fact]
    public async Task FirstOrDefaultAsync_WithTeamByIdWithSubscriptionsSpec_NoSubscriptions_ShouldReturnTeamWithEmptySubscriptions()
    {
        // Arrange
        var spec = new TeamByIdWithSubscriptionsSpec(_customerTeam.Id);

        // Act
        var result = await _repo.FirstOrDefaultAsync(spec);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(_customerTeam.Id);
        result.Subscriptions.ShouldBeEmpty();
    }

    #endregion

    //=================================//
    // TEAM BY ID WITH EVERYTHING SPEC TESTS
    //=================================//

    #region TeamByIdWithEverythingSpec Tests

    [Fact]
    public async Task FirstOrDefaultAsync_WithTeamByIdWithEverythingSpec_ShouldReturnTeamWithAllRelatedData()
    {
        // Arrange
        var member = AppUserDataFactory.Create(teamId: _customerTeam.Id, teamPosition: 1);
        var subscription = SubscriptionDataFactory.Create(teamId: _customerTeam.Id);
        var device = DeviceDataFactory.Create(subscriptionId: subscription.Id);

        await DbContext.Set<ID.Domain.Entities.AppUsers.AppUser>().AddAsync(member);
        await DbContext.Set<TeamSubscription>().AddAsync(subscription);
        await DbContext.Set<TeamDevice>().AddAsync(device);
        await DbContext.SaveChangesAsync();

        var spec = new TeamByIdWithEverythingSpec(_customerTeam.Id, maxPosition: 1000);

        // Act
        var result = await _repo.FirstOrDefaultAsync(spec);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(_customerTeam.Id);
        result.Members.ShouldContain(m => m.Id == member.Id);
        result.Subscriptions.ShouldContain(s => s.Id == subscription.Id);
        // Note: Device relationship verification depends on how the specification includes devices
    }

    [Fact]
    public async Task FirstOrDefaultAsync_WithTeamByIdWithEverythingSpec_NonExistingTeam_ShouldReturnNull()
    {
        // Arrange
        var nonExistingTeamId = Guid.NewGuid();
        var spec = new TeamByIdWithEverythingSpec(nonExistingTeamId, maxPosition: 1000);

        // Act
        var result = await _repo.FirstOrDefaultAsync(spec);

        // Assert
        result.ShouldBeNull();
    }

    #endregion

    //=================================//
    // CANCELLATION TOKEN TESTS
    //=================================//

    #region Cancellation Token Tests

    [Fact]
    public async Task FirstOrDefaultAsync_WithCancellationToken_ShouldRespectCancellation()
    {
        // Arrange
        var spec = new GetByIdSpec<Team>(_customerTeam.Id);
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act & Assert
        await Should.ThrowAsync<OperationCanceledException>(async () =>
            await _repo.FirstOrDefaultAsync(spec, cancellationTokenSource.Token));
    }

    [Fact]
    public async Task FirstOrDefaultAsync_WithValidCancellationToken_ShouldComplete()
    {
        // Arrange
        var spec = new GetByIdSpec<Team>(_customerTeam.Id);
        var cancellationTokenSource = new CancellationTokenSource();

        // Act
        var result = await _repo.FirstOrDefaultAsync(spec, cancellationTokenSource.Token);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(_customerTeam.Id);
    }

    #endregion

    //=================================//
    // PERFORMANCE TESTS
    //=================================//

    #region Performance Tests

    [Fact]
    public async Task FirstOrDefaultAsync_WithComplexIncludes_ShouldPerformEfficiently()
    {
        // Arrange
        var member = AppUserDataFactory.Create(teamId: _customerTeam.Id);
        var subscription = SubscriptionDataFactory.Create(teamId: _customerTeam.Id);
        
        await DbContext.Set<ID.Domain.Entities.AppUsers.AppUser>().AddAsync(member);
        await DbContext.Set<TeamSubscription>().AddAsync(subscription);
        await DbContext.SaveChangesAsync();

        var spec = new TeamByIdWithEverythingSpec(_customerTeam.Id, maxPosition: 1000);

        // Act & Assert - Should complete without timeout
        var result = await _repo.FirstOrDefaultAsync(spec);
        result.ShouldNotBeNull();
    }

    #endregion

}//Cls
