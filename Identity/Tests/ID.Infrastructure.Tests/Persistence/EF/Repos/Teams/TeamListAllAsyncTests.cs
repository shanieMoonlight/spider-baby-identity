using ID.Domain.Entities.Teams;
using ID.Infrastructure.Persistance.EF.Repos;
using ID.Infrastructure.Persistance.EF.Repos.Specs.Teams;
using ID.Tests.Data.Factories;
using Shouldly;

namespace ID.Infrastructure.Tests.Persistence.EF.Repos.Teams;

public class TeamListAllAsyncTests : RepoTestBase, IAsyncLifetime
{
    private TeamRepo _repo = null!;

    // Test data
    private readonly Team _customerTeam1 = TeamDataFactory.Create(
        name: "Alpha Customer Team",
        teamType: TeamType.Customer);

    private readonly Team _customerTeam2 = TeamDataFactory.Create(
        name: "Beta Customer Team", 
        teamType: TeamType.Customer);

    private readonly Team _maintenanceTeam = TeamDataFactory.Create(
        name: "Maintenance Team",
        teamType: TeamType.Maintenance);

    private readonly Team _superTeam = TeamDataFactory.Create(
        name: "Super Team",
        teamType: TeamType.Super);

    private readonly List<Team> _allTeams;

    //-----------------------------//

    public TeamListAllAsyncTests()
    {
        _allTeams = [_customerTeam1, _customerTeam2, _maintenanceTeam, _superTeam];
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
        await DbContext.Teams.AddRangeAsync(_allTeams);
        await DbContext.SaveChangesAsync();
    }

    //=================================//
    // ALL TEAMS SPEC TESTS
    //=================================//

    #region AllTeamsSpec Tests

    [Fact]
    public async Task ListAllAsync_WithAllTeamsSpec_ShouldReturnAllTeams()
    {
        // Arrange
        var spec = new AllTeamsSpec();

        // Act
        var result = await _repo.ListAllAsync(spec);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(4);
        result.ShouldContain(t => t.Id == _customerTeam1.Id);
        result.ShouldContain(t => t.Id == _customerTeam2.Id);
        result.ShouldContain(t => t.Id == _maintenanceTeam.Id);
        result.ShouldContain(t => t.Id == _superTeam.Id);
    }

    [Fact]
    public async Task ListAllAsync_WithAllTeamsSpec_ShouldIncludeAllTeamTypes()
    {
        // Arrange
        var spec = new AllTeamsSpec();

        // Act
        var result = await _repo.ListAllAsync(spec);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldContain(t => t.TeamType == TeamType.Customer);
        result.ShouldContain(t => t.TeamType == TeamType.Maintenance);
        result.ShouldContain(t => t.TeamType == TeamType.Super);
    }

    [Fact]
    public async Task ListAllAsync_WithAllTeamsSpec_EmptyDatabase_ShouldReturnEmptyList()
    {
        // Arrange
        await DbContext.Database.EnsureDeletedAsync();
        await DbContext.Database.EnsureCreatedAsync();
        var spec = new AllTeamsSpec();

        // Act
        var result = await _repo.ListAllAsync(spec);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeEmpty();
    }

    #endregion

    //=================================//
    // CUSTOMER TEAMS BY NAME SPEC TESTS
    //=================================//

    #region CustomerTeamsByNameSpec Tests

    [Fact]
    public async Task ListAllAsync_WithCustomerTeamsByNameSpec_ValidName_ShouldReturnMatchingTeams()
    {
        // Arrange
        var spec = new CustomerTeamsByNameSpec("Alpha");

        // Act
        var result = await _repo.ListAllAsync(spec);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(1);
        result.ShouldContain(t => t.Name.Contains("Alpha") && t.TeamType == TeamType.Customer);
        result.ShouldNotContain(t => t.TeamType != TeamType.Customer);
    }

    [Fact]
    public async Task ListAllAsync_WithCustomerTeamsByNameSpec_PartialMatch_ShouldReturnMatchingTeams()
    {
        // Arrange
        var spec = new CustomerTeamsByNameSpec("Customer");

        // Act
        var result = await _repo.ListAllAsync(spec);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);
        result.ShouldContain(t => t.Id == _customerTeam1.Id);
        result.ShouldContain(t => t.Id == _customerTeam2.Id);
        result.ShouldNotContain(t => t.TeamType != TeamType.Customer);
    }

    [Fact]
    public async Task ListAllAsync_WithCustomerTeamsByNameSpec_CaseInsensitive_ShouldReturnMatchingTeams()
    {
        // Arrange
        var spec = new CustomerTeamsByNameSpec("ALPHA");

        // Act
        var result = await _repo.ListAllAsync(spec);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(1);
        result.ShouldContain(t => t.Name.Contains("Alpha"));
    }

    [Fact]
    public async Task ListAllAsync_WithCustomerTeamsByNameSpec_NonExistentName_ShouldReturnEmpty()
    {
        // Arrange
        var spec = new CustomerTeamsByNameSpec("NonExistent");

        // Act
        var result = await _repo.ListAllAsync(spec);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task ListAllAsync_WithCustomerTeamsByNameSpec_EmptyName_ShouldShortCircuit()
    {
        // Arrange
        var spec = new CustomerTeamsByNameSpec("");

        // Act
        var result = await _repo.ListAllAsync(spec);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeEmpty(); // Short circuit should return empty
    }

    [Fact]
    public async Task ListAllAsync_WithCustomerTeamsByNameSpec_NullName_ShouldShortCircuit()
    {
        // Arrange
        var spec = new CustomerTeamsByNameSpec(null);

        // Act
        var result = await _repo.ListAllAsync(spec);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeEmpty(); // Short circuit should return empty
    }

    [Fact]
    public async Task ListAllAsync_WithCustomerTeamsByNameSpec_OnlyIncludesCustomerTeams()
    {
        // Arrange
        var spec = new CustomerTeamsByNameSpec("Team"); // Should match all teams by name

        // Act
        var result = await _repo.ListAllAsync(spec);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldAllBe(t => t.TeamType == TeamType.Customer);
        result.ShouldNotContain(t => t.TeamType == TeamType.Maintenance);
        result.ShouldNotContain(t => t.TeamType == TeamType.Super);
    }

    #endregion

    //=================================//
    // TEAMS WITH EXPIRED SUBSCRIPTIONS SPEC TESTS
    //=================================//

    #region TeamsWithExpiredSubscriptionsSpec Tests

    [Fact]
    public async Task ListAllAsync_WithTeamsWithExpiredSubscriptionsSpec_NoExpiredSubscriptions_ShouldReturnEmpty()
    {
        // Arrange
        var spec = new TeamsWithExpiredSubscriptionsSpec();

        // Act
        var result = await _repo.ListAllAsync(spec);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeEmpty(); // No teams have expired subscriptions in test data
    }

    [Fact]
    public async Task ListAllAsync_WithTeamsWithExpiredSubscriptionsSpec_WithExpiredSubscription_ShouldReturnTeam()
    {
        // Arrange
        var expiredSubscription = SubscriptionDataFactory.Create(
            teamId: _customerTeam1.Id,
            trial: false,
            endDate: DateTime.UtcNow.AddDays(-2));
        
        // Set end date to past to make it expired
        //var endDateField = typeof(TeamSubscription).GetField("EndDate",
        //    BindingFlags.NonPublic | BindingFlags.Instance);
        //endDateField?.SetValue(expiredSubscription, DateTime.UtcNow.AddDays(-1));

        // Use reflection to add subscription to team since AddSubscription requires validation token
        var subscriptionsField = typeof(Team).GetField("_subscriptions",
            BindingFlags.NonPublic | BindingFlags.Instance);
        var subscriptions = (HashSet<TeamSubscription>)subscriptionsField!.GetValue(_customerTeam1)!;
        subscriptions.Add(expiredSubscription);

        await DbContext.Set<TeamSubscription>().AddAsync(expiredSubscription);

        await DbContext.SaveChangesAsync();

        var spec = new TeamsWithExpiredSubscriptionsSpec();

        // Act
        var result = await _repo.ListAllAsync(spec);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(1);
        result.ShouldContain(t => t.Id == _customerTeam1.Id);
    }

    [Fact]
    public async Task ListAllAsync_WithTeamsWithExpiredSubscriptionsSpec_WithActiveSubscription_ShouldReturnEmpty()
    {
        // Arrange
        var activeSubscription = SubscriptionDataFactory.Create(
            teamId: _customerTeam1.Id,
            trial: false);
        
        // Set end date to future to make it active
        var endDateField = typeof(TeamSubscription).GetField("EndDate",
            BindingFlags.NonPublic | BindingFlags.Instance);
        endDateField?.SetValue(activeSubscription, DateTime.UtcNow.AddDays(30));

        await DbContext.Set<TeamSubscription>().AddAsync(activeSubscription);
        await DbContext.SaveChangesAsync();

        var spec = new TeamsWithExpiredSubscriptionsSpec();

        // Act
        var result = await _repo.ListAllAsync(spec);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeEmpty(); // No expired subscriptions
    }

    #endregion

    //=================================//
    // CANCELLATION TOKEN TESTS
    //=================================//

    #region Cancellation Token Tests

    [Fact]
    public async Task ListAllAsync_WithCancellationToken_ShouldRespectCancellation()
    {
        // Arrange
        var spec = new AllTeamsSpec();
        var cts = new CancellationTokenSource();
        cts.Cancel(); // Cancel immediately

        // Act & Assert
        await Should.ThrowAsync<OperationCanceledException>(async () =>
            await _repo.ListAllAsync(spec, cts.Token));
    }

    [Fact]
    public async Task ListAllAsync_WithValidCancellationToken_ShouldComplete()
    {
        // Arrange
        var spec = new AllTeamsSpec();
        var cts = new CancellationTokenSource();

        // Act
        var result = await _repo.ListAllAsync(spec, cts.Token);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(4);
    }

    #endregion

    //=================================//
    // SPECIFICATION BEHAVIOR TESTS
    //=================================//

    #region Specification Behavior Tests

    [Fact]
    public async Task ListAllAsync_WithMultipleSpecs_ShouldBehaveDifferently()
    {
        // Arrange
        var allTeamsSpec = new AllTeamsSpec();
        var customerTeamsSpec = new CustomerTeamsByNameSpec("Team");

        // Act
        var allResults = await _repo.ListAllAsync(allTeamsSpec);
        var customerResults = await _repo.ListAllAsync(customerTeamsSpec);

        // Assert
        allResults.Count.ShouldBeGreaterThan(customerResults.Count);
        allResults.ShouldContain(t => t.TeamType == TeamType.Maintenance);
        customerResults.ShouldNotContain(t => t.TeamType == TeamType.Maintenance);
    }

    [Fact]
    public async Task ListAllAsync_SpecWithIncludes_ShouldLoadRelatedData()
    {
        // Arrange
        var spec = new CustomerTeamsByNameSpec("Alpha");

        // Act
        var result = await _repo.ListAllAsync(spec);

        // Assert
        result.ShouldNotBeNull();
        var team = result.First();
        
        // The spec includes Members, so they should be loaded
        // Note: Members collection should not be null even if empty
        team.Members.ShouldNotBeNull();
    }

    #endregion

}//Cls
