using ID.Domain.Entities.Teams;
using ID.Domain.Utility.Exceptions;
using ID.Infrastructure.Persistance.EF.Repos;
using ID.Tests.Data.Factories;
using MyResults;
using Shouldly;
using System.Reflection;

namespace ID.Infrastructure.Tests.Persistence.EF.Repos.Teams;

public class TeamDeleteAsyncTests : RepoTestBase, IAsyncLifetime
{
    private TeamRepo _repo = null!;

    //-----------------------------//

    public async Task InitializeAsync()
    {
        _repo = new TeamRepo(DbContext);
        await Task.CompletedTask;
    }

    public Task DisposeAsync() => Task.CompletedTask;

    //=================================//
    // DELETE ASYNC BY ID TESTS
    //=================================//

    #region Delete By ID Tests

    [Fact]
    public async Task DeleteAsync_WithValidCustomerTeamId_ShouldDeleteTeam()
    {
        // Arrange
        var team = TeamDataFactory.Create(teamType: TeamType.Customer);
        await _repo.AddAsync(team);
        await DbContext.SaveChangesAsync();

        // Act
        await _repo.DeleteAsync(team.Id);
        await DbContext.SaveChangesAsync();

        // Assert
        var deletedTeam = await DbContext.Teams.FindAsync(team.Id);
        deletedTeam.ShouldBeNull();
    }

    [Fact]
    public async Task DeleteAsync_WithNullId_ShouldNotThrowAndReturnGracefully()
    {
        // Arrange
        Guid? nullId = null;

        // Act & Assert - Should not throw
        await Should.NotThrowAsync(async () =>
            await _repo.DeleteAsync(nullId));
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentId_ShouldNotThrowAndReturnGracefully()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act & Assert - Should not throw (already deleted scenario)
        await Should.NotThrowAsync(async () =>
            await _repo.DeleteAsync(nonExistentId));
    }

    [Fact]
    public async Task DeleteAsync_WithSuperTeamId_ShouldThrowCantDeleteException()
    {
        // Arrange
        var superTeam = TeamDataFactory.Create(teamType: TeamType.Super);
        await _repo.AddAsync(superTeam);
        await DbContext.SaveChangesAsync();

        // Act & Assert
        var exception = await Should.ThrowAsync<CantDeleteException>(async () =>
            await _repo.DeleteAsync(superTeam.Id));

        exception.Message.ShouldContain("Team");
    }

    [Fact]
    public async Task DeleteAsync_WithMaintenanceTeamId_ShouldThrowCantDeleteException()
    {
        // Arrange
        var maintenanceTeam = TeamDataFactory.Create(teamType: TeamType.Maintenance);
        await _repo.AddAsync(maintenanceTeam);
        await DbContext.SaveChangesAsync();

        // Act & Assert
        var exception = await Should.ThrowAsync<CantDeleteException>(async () =>
            await _repo.DeleteAsync(maintenanceTeam.Id));

        exception.Message.ShouldContain("Team");
    }

    [Fact]
    public async Task DeleteAsync_WithCustomerTeamWithMembers_ShouldThrowCantDeleteException()
    {
        // Arrange
        var leader = AppUserDataFactory.Create();
        var member = AppUserDataFactory.Create();
        var team = TeamDataFactory.Create(
            teamType: TeamType.Customer,
            leader: leader,
            members: [leader, member]);

        await _repo.AddAsync(team);
        await DbContext.SaveChangesAsync();

        // Act & Assert
        var exception = await Should.ThrowAsync<CantDeleteException>(async () =>
            await _repo.DeleteAsync(team.Id));

        exception.Message.ShouldNotBeNull();
    }

    #endregion

    //=================================//
    // DELETE ASYNC BY ENTITY TESTS
    //=================================//

    #region Delete By Entity Tests

    [Fact]
    public async Task DeleteAsync_WithValidCustomerTeamEntity_ShouldDeleteTeam()
    {
        // Arrange
        var team = TeamDataFactory.Create(teamType: TeamType.Customer);
        await _repo.AddAsync(team);
        await DbContext.SaveChangesAsync();

        // Act
        await _repo.DeleteAsync(team);
        await DbContext.SaveChangesAsync();

        // Assert
        var deletedTeam = await DbContext.Teams.FindAsync(team.Id);
        deletedTeam.ShouldBeNull();
    }

    [Fact]
    public async Task DeleteAsync_WithSuperTeamEntity_ShouldThrowCantDeleteException()
    {
        // Arrange
        var superTeam = TeamDataFactory.Create(teamType: TeamType.Super);

        // Act & Assert
        var exception = await Should.ThrowAsync<CantDeleteException>(async () =>
            await _repo.DeleteAsync(superTeam));

        exception.Message.ShouldContain("Team");
    }

    [Fact]
    public async Task DeleteAsync_WithMaintenanceTeamEntity_ShouldThrowCantDeleteException()
    {
        // Arrange
        var maintenanceTeam = TeamDataFactory.Create(teamType: TeamType.Maintenance);

        // Act & Assert
        var exception = await Should.ThrowAsync<CantDeleteException>(async () =>
            await _repo.DeleteAsync(maintenanceTeam));

        exception.Message.ShouldContain("Team");
    }

    #endregion

    //=================================//
    // CAN DELETE TESTS (Using Reflection)
    //=================================//

    #region Can Delete Tests

    [Fact]
    public async Task CanDeleteAsync_WithNullTeam_ShouldReturnSuccess()
    {
        // Arrange
        Team? nullTeam = null;

        // Use reflection to access protected method for testing
        var canDeleteMethod = typeof(TeamRepo)
            .GetMethod("CanDeleteAsync", BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var result = await (Task<BasicResult>)canDeleteMethod!.Invoke(_repo, [nullTeam])!;

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeTrue();
    }

    [Fact]
    public async Task CanDeleteAsync_WithSuperTeam_ShouldReturnFailure()
    {
        // Arrange
        var superTeam = TeamDataFactory.Create(teamType: TeamType.Super);

        // Use reflection to access protected method for testing
        var canDeleteMethod = typeof(TeamRepo)
            .GetMethod("CanDeleteAsync", BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var result = await (Task<BasicResult>)canDeleteMethod!.Invoke(_repo, [superTeam])!;

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldNotBeNull();
    }

    [Fact]
    public async Task CanDeleteAsync_WithMaintenanceTeam_ShouldReturnFailure()
    {
        // Arrange
        var maintenanceTeam = TeamDataFactory.Create(teamType: TeamType.Maintenance);

        // Use reflection to access protected method for testing
        var canDeleteMethod = typeof(TeamRepo)
            .GetMethod("CanDeleteAsync", BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var result = await (Task<BasicResult>)canDeleteMethod!.Invoke(_repo, [maintenanceTeam])!;

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldNotBeNull();
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

    [Fact]
    public async Task CanDeleteAsync_WithCustomerTeamWithOnlyLeader_ShouldReturnSuccess()
    {
        // Arrange
        var leader = AppUserDataFactory.Create();
        var customerTeam = TeamDataFactory.Create(
            teamType: TeamType.Customer,
            leader: leader,
            members: [leader]);

        // Use reflection to access protected method for testing
        var canDeleteMethod = typeof(TeamRepo)
            .GetMethod("CanDeleteAsync", BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var result = await (Task<BasicResult>)canDeleteMethod!.Invoke(_repo, [customerTeam])!;

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeTrue();
    }

    #endregion

    //=================================//
    // REMOVE RANGE TESTS
    //=================================//

    #region Remove Range Tests

    [Fact]
    public async Task RemoveRangeAsync_WithMultipleValidCustomerTeams_ShouldRemoveAll()
    {
        // Arrange
        var team1 = TeamDataFactory.Create(name: "Team 1", teamType: TeamType.Customer);
        var team2 = TeamDataFactory.Create(name: "Team 2", teamType: TeamType.Customer);
        var team3 = TeamDataFactory.Create(name: "Team 3", teamType: TeamType.Customer);

        await _repo.AddAsync(team1);
        await _repo.AddAsync(team2);
        await _repo.AddAsync(team3);
        await DbContext.SaveChangesAsync();

        var teamsToRemove = new[] { team1, team2, team3 };

        // Act
        await _repo.RemoveRangeAsync(teamsToRemove);
        await DbContext.SaveChangesAsync();

        // Assert
        var remainingTeam1 = await DbContext.Teams.FindAsync(team1.Id);
        var remainingTeam2 = await DbContext.Teams.FindAsync(team2.Id);
        var remainingTeam3 = await DbContext.Teams.FindAsync(team3.Id);

        remainingTeam1.ShouldBeNull();
        remainingTeam2.ShouldBeNull();
        remainingTeam3.ShouldBeNull();
    }

    [Fact]
    public async Task RemoveRangeAsync_WithEmptyCollection_ShouldNotThrow()
    {
        // Arrange
        var emptyTeams = new Team[0];

        // Act & Assert
        await Should.NotThrowAsync(async () =>
            await _repo.RemoveRangeAsync(emptyTeams));
    }

    [Fact]
    public async Task RemoveRangeAsync_WithSpecification_ShouldRemoveMatchingTeams()
    {
        // Arrange
        var customerTeam1 = TeamDataFactory.Create(name: "Customer A", teamType: TeamType.Customer);
        var customerTeam2 = TeamDataFactory.Create(name: "Customer B", teamType: TeamType.Customer);
        var maintenanceTeam = TeamDataFactory.Create(name: "Maintenance", teamType: TeamType.Maintenance);

        await _repo.AddAsync(customerTeam1);
        await _repo.AddAsync(customerTeam2);
        await _repo.AddAsync(maintenanceTeam);
        await DbContext.SaveChangesAsync();

        // Create a simple spec that matches customer teams
        var spec = new GetTeamsByTypeSpec(TeamType.Customer);

        // Act
        await _repo.RemoveRangeAsync(spec);
        await DbContext.SaveChangesAsync();

        // Assert
        var remainingCustomer1 = await DbContext.Teams.FindAsync(customerTeam1.Id);
        var remainingCustomer2 = await DbContext.Teams.FindAsync(customerTeam2.Id);
        var remainingMaintenance = await DbContext.Teams.FindAsync(maintenanceTeam.Id);

        remainingCustomer1.ShouldBeNull();
        remainingCustomer2.ShouldBeNull();
        remainingMaintenance.ShouldNotBeNull(); // Should still exist
    }

    #endregion

    //=================================//
    // PERSISTENCE VERIFICATION TESTS
    //=================================//

    #region Persistence Tests

    [Fact]
    public async Task DeleteAsync_ShouldPersistDeletionAfterSaveChanges()
    {
        // Arrange
        var team = TeamDataFactory.Create(teamType: TeamType.Customer);
        await _repo.AddAsync(team);
        await DbContext.SaveChangesAsync();

        // Verify team exists
        var existingTeam = await DbContext.Teams.FindAsync(team.Id);
        existingTeam.ShouldNotBeNull();

        // Act
        await _repo.DeleteAsync(team.Id);
        await DbContext.SaveChangesAsync();

        // Assert
        var deletedTeam = await DbContext.Teams.FindAsync(team.Id);        deletedTeam.ShouldBeNull();
    }

    #endregion

}//Cls
