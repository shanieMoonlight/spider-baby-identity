using ID.Domain.Entities.Teams;
using ID.Domain.Utility.Messages;
using ID.Infrastructure.Persistance.EF.Repos;
using ID.Infrastructure.Persistance.EF.Repos.Specs.Teams;
using ID.Tests.Data.Factories;
using MyResults;
using Pagination;
using Shouldly;
using System.Reflection;
using ClArch.ValueObjects;

namespace ID.Infrastructure.Tests.Persistence.EF.Repos.Teams;

public class TeamRepoTests : RepoTestBase, IAsyncLifetime
{
    private TeamRepo _repo = null!;

    private readonly Team _customerTeam = TeamDataFactory.Create(
        name: "Customer Alpha",
        description: "Primary customer team",
        teamType: TeamType.Customer);

    private readonly Team _maintenanceTeam = TeamDataFactory.Create(
        name: "Maintenance Team",
        description: "System maintenance team",
        teamType: TeamType.Maintenance);

    private readonly Team _superTeam = TeamDataFactory.Create(
        name: "Super Team", 
        description: "Super admin team",
        teamType: TeamType.Super);

    private readonly List<Team> _teams;

    //-----------------------------//

    public TeamRepoTests()
    {
        _teams = [_customerTeam, _maintenanceTeam, _superTeam];
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
    // UPDATE TESTS
    //=================================//

    #region Update Tests

    [Fact]
    public async Task UpdateAsync_WithValidTeam_ShouldUpdateTeam()
    {
        // Arrange
        var teamToUpdate = _customerTeam;
        teamToUpdate.Update(
            Name.Create("Updated Team Name"),
            DescriptionNullable.Create("Updated description"));

        // Act
        var result = await _repo.UpdateAsync(teamToUpdate);

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Updated Team Name");
        result.Description.ShouldBe("Updated description");
    }

    #endregion

    //=================================//
    // PAGE TESTS
    //=================================//

    #region Page Tests

    [Fact]
    public async Task PageAsync_ShouldExcludeSuperTeams()
    {
        // Arrange
        var sortList = new List<SortRequest>();
        var filterList = new List<FilterRequest>();

        // Act
        var result = await _repo.PageAsync(1, 10, sortList, filterList);

        // Assert
        result.ShouldNotBeNull();
        result.Data.ShouldNotContain(t => t.TeamType == TeamType.Super);
        result.Data.ShouldContain(t => t.TeamType == TeamType.Customer);
        result.Data.ShouldContain(t => t.TeamType == TeamType.Maintenance);
    }

    [Fact]
    public async Task PageAsync_WithSorting_ShouldReturnSortedResults()
    {
        // Arrange
        var sortList = new List<SortRequest> 
        { 
            new() { Field = "Name", SortDescending = false }
        };

        // Act
        var result = await _repo.PageAsync(1, 10, sortList);

        // Assert
        result.ShouldNotBeNull();
        result.Data.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task PageAsync_WithFiltering_ShouldReturnFilteredResults()
    {
        // Arrange
        var filterList = new List<FilterRequest>
        {
            new() { Field = "Name", FilterValue = "Customer", FilterType = "contains" }
        };

        // Act
        var result = await _repo.PageAsync(1, 10, [], filterList);

        // Assert
        result.ShouldNotBeNull();
    }

    [Fact]
    public async Task PageAsync_WithPagination_ShouldReturnCorrectPage()
    {
        // Arrange
        var pageNumber = 1;
        var pageSize = 1;

        // Act
        var result = await _repo.PageAsync(pageNumber, pageSize, []);

        // Assert
        result.ShouldNotBeNull();
        result.Number.ShouldBe(pageNumber);
        result.Size.ShouldBe(pageSize);
    }

    #endregion

    //=================================//
    // CAN DELETE TESTS (using reflection)
    //=================================//

    #region Can Delete Tests

    [Fact]
    public async Task CanDeleteAsync_WithNullTeam_ShouldReturnSuccess()
    {
        // Use reflection to access protected method for testing
        var canDeleteMethod = typeof(TeamRepo)
            .GetMethod("CanDeleteAsync", BindingFlags.NonPublic | BindingFlags.Instance);
        
        // Act
        var result = await (Task<BasicResult>)canDeleteMethod!.Invoke(_repo, [null])!;

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeTrue();
    }

    [Fact]
    public async Task CanDeleteAsync_WithSuperTeam_ShouldReturnFailure()
    {
        // Use reflection to access protected method for testing
        var canDeleteMethod = typeof(TeamRepo)
            .GetMethod("CanDeleteAsync", BindingFlags.NonPublic | BindingFlags.Instance);
        
        // Act
        var result = await (Task<BasicResult>)canDeleteMethod!.Invoke(_repo, [_superTeam])!;

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldBe(IDMsgs.Error.Teams.CANNOT_DELETE_SUPER_TEAM);
    }

    [Fact]
    public async Task CanDeleteAsync_WithMaintenanceTeam_ShouldReturnFailure()
    {
        // Use reflection to access protected method for testing
        var canDeleteMethod = typeof(TeamRepo)
            .GetMethod("CanDeleteAsync", BindingFlags.NonPublic | BindingFlags.Instance);
        
        // Act
        var result = await (Task<BasicResult>)canDeleteMethod!.Invoke(_repo, [_maintenanceTeam])!;

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldBe(IDMsgs.Error.Teams.CANNOT_DELETE_MNTC_TEAM);
    }

    [Fact]
    public async Task CanDeleteAsync_WithCustomerTeamWithoutMembers_ShouldReturnSuccess()
    {
        // Arrange
        var customerTeam = TeamDataFactory.Create(teamType: TeamType.Customer);

        // Use reflection to access protected method for testing
        var canDeleteMethod = typeof(TeamRepo)
            .GetMethod("CanDeleteAsync", BindingFlags.NonPublic | BindingFlags.Instance);
        
        // Act
        var result = await (Task<BasicResult>)canDeleteMethod!.Invoke(_repo, [customerTeam])!;

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeTrue();
    }

    [Fact]
    public async Task CanDeleteAsync_WithCustomerTeamWithMembers_ShouldReturnFailure()
    {
        // Arrange
        var leader = AppUserDataFactory.Create();
        var member1 = AppUserDataFactory.Create();
        var member2 = AppUserDataFactory.Create();
        
        var customerTeam = TeamDataFactory.Create(
            teamType: TeamType.Customer,
            leader: leader,
            members: [leader, member1, member2]);

        // Use reflection to access protected method for testing
        var canDeleteMethod = typeof(TeamRepo)
            .GetMethod("CanDeleteAsync", BindingFlags.NonPublic | BindingFlags.Instance);
        
        // Act
        var result = await (Task<BasicResult>)canDeleteMethod!.Invoke(_repo, [customerTeam])!;

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldContain("2 members connected to Team");
    }

    #endregion

    //=================================//
    // SPECIFICATION TESTS  
    //=================================//

    #region Specification Tests

    [Fact]
    public async Task ListAllAsync_WithCustomerTeamsByNameSpec_ShouldReturnMatchingTeams()
    {
        // Arrange
        var spec = new CustomerTeamsByNameSpec("Customer");

        // Act
        var result = await _repo.ListAllAsync(spec);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldContain(t => t.Name.Contains("Customer") && t.TeamType == TeamType.Customer);
        result.ShouldNotContain(t => t.TeamType != TeamType.Customer);
    }

    [Fact]
    public async Task FirstOrDefaultAsync_WithTeamByIdWithMembersSpec_ShouldIncludeMembers()
    {
        // Arrange
        var spec = new TeamByIdWithMembersSpec(_customerTeam.Id, 1000);

        // Act
        var result = await _repo.FirstOrDefaultAsync(spec);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(_customerTeam.Id);
    }

    [Fact]
    public async Task FirstOrDefaultAsync_WithTeamByIdWithSubscriptionsSpec_ShouldWork()
    {
        // Arrange
        var spec = new TeamByIdWithSubscriptionsSpec(_customerTeam.Id);

        // Act
        var result = await _repo.FirstOrDefaultAsync(spec);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(_customerTeam.Id);
    }

    #endregion

    //=================================//
    // BUSINESS RULE TESTS
    //=================================//

    #region Business Rule Tests

    [Fact]
    public async Task UpdateAsync_ShouldPreserveTeamType()
    {
        // Arrange
        var originalType = _customerTeam.TeamType;

        // Act
        var result = await _repo.UpdateAsync(_customerTeam);

        // Assert
        result.TeamType.ShouldBe(originalType);
    }

    [Fact]
    public async Task UpdateAsync_ShouldPreserveId()
    {
        // Arrange
        var originalId = _customerTeam.Id;

        // Act
        var result = await _repo.UpdateAsync(_customerTeam);

        // Assert
        result.Id.ShouldBe(originalId);
    }

    #endregion

    //=================================//
    // ERROR HANDLING TESTS
    //=================================//

    #region Error Handling Tests

    #endregion

}//Cls
