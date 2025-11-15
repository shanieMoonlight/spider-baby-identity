using ID.Domain.Entities.Teams;
using ID.Infrastructure.Persistance.EF.Repos;
using ID.Tests.Data.Factories;
using Shouldly;
using ClArch.ValueObjects;

namespace ID.Infrastructure.Tests.Persistence.EF.Repos.Teams;

public class TeamUpdateAsyncTests : RepoTestBase, IAsyncLifetime
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
    // UPDATE ASYNC TESTS
    //=================================//

    #region Basic Update Tests

    [Fact]
    public async Task UpdateAsync_WithValidTeam_ShouldUpdateTeam()
    {
        // Arrange
        var team = TeamDataFactory.Create(
            name: "Original Team",
            description: "Original description",
            teamType: TeamType.customer);

        await _repo.AddAsync(team);
        await DbContext.SaveChangesAsync();

        // Update using domain method
        team.Update(
            Name.Create("Updated Team Name"),
            DescriptionNullable.Create("Updated description"));

        // Act
        var result = await _repo.UpdateAsync(team);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(team.Id);
        result.Name.ShouldBe("Updated Team Name");
        result.Description.ShouldBe("Updated description");
        result.TeamType.ShouldBe(TeamType.customer); // Should remain unchanged
    }

    [Fact]
    public async Task UpdateAsync_WithNullDescription_ShouldUpdateToNull()
    {
        // Arrange
        var team = TeamDataFactory.Create(
            name: "Test Team",
            description: "Has description",
            teamType: TeamType.customer);

        await _repo.AddAsync(team);
        await DbContext.SaveChangesAsync();

        // Update to null description
        team.Update(
            Name.Create("Test Team"),
            DescriptionNullable.Create(null));

        // Act
        var result = await _repo.UpdateAsync(team);

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Test Team");
        result.Description.ShouldBeNull();
    }

    [Fact]
    public async Task UpdateAsync_WithEmptyDescription_ShouldUpdateToEmpty()
    {
        // Arrange
        var team = TeamDataFactory.Create(
            name: "Test Team",
            description: "Has description",
            teamType: TeamType.customer);

        await _repo.AddAsync(team);
        await DbContext.SaveChangesAsync();

        // Update to empty description
        team.Update(
            Name.Create("Test Team"),
            DescriptionNullable.Create(""));

        // Act
        var result = await _repo.UpdateAsync(team);

        // Assert
        result.ShouldNotBeNull();        result.Name.ShouldBe("Test Team");
        result.Description.ShouldBe("");
    }

    #endregion

    //=================================//
    // TEAM TYPE PRESERVATION TESTS
    //=================================//

    #region Team Type Tests

    [Fact]
    public async Task UpdateAsync_CustomerTeam_ShouldPreserveTeamType()
    {
        // Arrange
        var team = TeamDataFactory.Create(teamType: TeamType.customer);
        await _repo.AddAsync(team);
        await DbContext.SaveChangesAsync();

        team.Update(
            Name.Create("Updated Customer Team"),
            DescriptionNullable.Create("Updated"));

        // Act
        var result = await _repo.UpdateAsync(team);

        // Assert
        result.TeamType.ShouldBe(TeamType.customer);
    }

    [Fact]
    public async Task UpdateAsync_MaintenanceTeam_ShouldPreserveTeamType()
    {
        // Arrange
        var team = TeamDataFactory.Create(teamType: TeamType.maintenance);
        await _repo.AddAsync(team);
        await DbContext.SaveChangesAsync();

        team.Update(
            Name.Create("Updated Maintenance Team"),
            DescriptionNullable.Create("Updated"));

        // Act
        var result = await _repo.UpdateAsync(team);

        // Assert
        result.TeamType.ShouldBe(TeamType.maintenance);
    }

    [Fact]
    public async Task UpdateAsync_SuperTeam_ShouldPreserveTeamType()
    {
        // Arrange
        var team = TeamDataFactory.Create(teamType: TeamType.super);
        await _repo.AddAsync(team);
        await DbContext.SaveChangesAsync();

        team.Update(
            Name.Create("Updated Super Team"),
            DescriptionNullable.Create("Updated"));

        // Act
        var result = await _repo.UpdateAsync(team);

        // Assert
        result.TeamType.ShouldBe(TeamType.super);
    }

    #endregion

    //=================================//
    // PROPERTY PRESERVATION TESTS
    //=================================//

    #region Property Preservation Tests

    [Fact]
    public async Task UpdateAsync_ShouldPreserveId()
    {
        // Arrange
        var team = TeamDataFactory.Create(teamType: TeamType.customer);
        var originalId = team.Id;
        await _repo.AddAsync(team);
        await DbContext.SaveChangesAsync();

        team.Update(
            Name.Create("Updated Team"),
            DescriptionNullable.Create("Updated"));

        // Act
        var result = await _repo.UpdateAsync(team);

        // Assert
        result.Id.ShouldBe(originalId);
    }

    [Fact]
    public async Task UpdateAsync_ShouldPreserveCapacity()
    {
        // Arrange
        var team = TeamDataFactory.Create(
            teamType: TeamType.customer,
            capacity: 25);
        await _repo.AddAsync(team);
        await DbContext.SaveChangesAsync();

        team.Update(
            Name.Create("Updated Team"),
            DescriptionNullable.Create("Updated"));

        // Act
        var result = await _repo.UpdateAsync(team);

        // Assert
        result.Capacity.ShouldBe(25);
    }

    [Fact]
    public async Task UpdateAsync_ShouldPreservePositionRange()
    {
        // Arrange
        var team = TeamDataFactory.Create(
            teamType: TeamType.customer,
            minPosition: 2,
            maxPosition: 8);
        await _repo.AddAsync(team);
        await DbContext.SaveChangesAsync();

        team.Update(
            Name.Create("Updated Team"),
            DescriptionNullable.Create("Updated"));

        // Act
        var result = await _repo.UpdateAsync(team);

        // Assert
        result.MinPosition.ShouldBe(2);
        result.MaxPosition.ShouldBe(8);
    }

    [Fact]
    public async Task UpdateAsync_WithLeader_ShouldPreserveLeader()
    {
        // Arrange
        var leader = AppUserDataFactory.Create();
        var team = TeamDataFactory.Create(
            teamType: TeamType.customer,
            leader: leader,
            members: [leader]);
        
        await _repo.AddAsync(team);
        await DbContext.SaveChangesAsync();

        team.Update(
            Name.Create("Updated Team"),
            DescriptionNullable.Create("Updated"));

        // Act
        var result = await _repo.UpdateAsync(team);

        // Assert
        result.LeaderId.ShouldBe(leader.Id);
        result.Leader?.Id.ShouldBe(leader.Id);
    }

    #endregion

    //=================================//
    // PERSISTENCE VERIFICATION TESTS
    //=================================//

    #region Persistence Tests

    [Fact]
    public async Task UpdateAsync_ShouldPersistAfterSaveChanges()
    {
        // Arrange
        var team = TeamDataFactory.Create(
            name: "Before Update",
            description: "Before description",
            teamType: TeamType.customer);

        await _repo.AddAsync(team);
        await DbContext.SaveChangesAsync();

        team.Update(
            Name.Create("After Update"),
            DescriptionNullable.Create("After description"));

        // Act
        var updatedTeam = await _repo.UpdateAsync(team);
        await DbContext.SaveChangesAsync();

        // Verify persistence by retrieving from database
        var persistedTeam = await DbContext.Teams.FindAsync(team.Id);

        // Assert
        updatedTeam.ShouldNotBeNull();
        persistedTeam.ShouldNotBeNull();
        persistedTeam.Id.ShouldBe(team.Id);
        persistedTeam.Name.ShouldBe("After Update");
        persistedTeam.Description.ShouldBe("After description");
    }

    [Fact]
    public async Task UpdateAsync_MultipleUpdates_ShouldPersistLatestChanges()
    {
        // Arrange
        var team = TeamDataFactory.Create(
            name: "Version 1",
            teamType: TeamType.customer);

        await _repo.AddAsync(team);
        await DbContext.SaveChangesAsync();

        // First update
        team.Update(
            Name.Create("Version 2"),
            DescriptionNullable.Create("Second version"));
        await _repo.UpdateAsync(team);
        await DbContext.SaveChangesAsync();

        // Second update
        team.Update(
            Name.Create("Version 3"),
            DescriptionNullable.Create("Third version"));

        // Act
        var result = await _repo.UpdateAsync(team);
        await DbContext.SaveChangesAsync();

        // Assert
        result.Name.ShouldBe("Version 3");
        result.Description.ShouldBe("Third version");

        // Verify persistence
        var persistedTeam = await DbContext.Teams.FindAsync(team.Id);
        persistedTeam!.Name.ShouldBe("Version 3");
        persistedTeam.Description.ShouldBe("Third version");
    }

    #endregion

    //=================================//
    // EDGE CASE TESTS
    //=================================//

    #region Edge Cases

    [Fact]
    public async Task UpdateAsync_WithSpecialCharacters_ShouldPreserveCharacters()
    {
        // Arrange
        var team = TeamDataFactory.Create(
            name: "Normal Team",
            teamType: TeamType.customer);

        await _repo.AddAsync(team);
        await DbContext.SaveChangesAsync();

        var specialName = "Team-With_Special@Characters#2024!";
        var specialDescription = "Description with émojis 🚀 and symbols ♠♥♦♣";
        
        team.Update(
            Name.Create(specialName),
            DescriptionNullable.Create(specialDescription));

        // Act
        var result = await _repo.UpdateAsync(team);

        // Assert
        result.Name.ShouldBe(specialName);
        result.Description.ShouldBe(specialDescription);
    }

    [Fact]
    public async Task UpdateAsync_WithUnchangedValues_ShouldStillWork()
    {
        // Arrange
        var team = TeamDataFactory.Create(
            name: "Unchanged Team",
            description: "Unchanged description",
            teamType: TeamType.customer);

        await _repo.AddAsync(team);
        await DbContext.SaveChangesAsync();

        // "Update" with same values
        team.Update(
            Name.Create("Unchanged Team"),
            DescriptionNullable.Create("Unchanged description"));

        // Act
        var result = await _repo.UpdateAsync(team);

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Unchanged Team");
        result.Description.ShouldBe("Unchanged description");
    }

    #endregion

    //=================================//
    // ERROR HANDLING TESTS
    //=================================//

    #region Error Handling

  

    [Fact]
    public async Task UpdateAsync_WithNonExistentTeam_ShouldStillWork()
    {
        // Arrange - Create team but don't add to database
        var team = TeamDataFactory.Create(teamType: TeamType.customer);
        
        team.Update(
            Name.Create("Updated Team"),
            DescriptionNullable.Create("Updated"));

        // Act - EF will treat this as an insert or update based on tracking
        var result = await _repo.UpdateAsync(team);

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Updated Team");
        result.Description.ShouldBe("Updated");
    }

    #endregion

    //=================================//
    // CONCURRENT UPDATE TESTS
    //=================================//

    #region Concurrency Tests

    [Fact]
    public async Task UpdateAsync_ConcurrentUpdates_ShouldHandleCorrectly()
    {
        // Arrange
        var team = TeamDataFactory.Create(
            name: "Concurrent Team",
            teamType: TeamType.customer);

        await _repo.AddAsync(team);
        await DbContext.SaveChangesAsync();

        // Simulate concurrent updates
        team.Update(
            Name.Create("First Update"),
            DescriptionNullable.Create("First"));

        var result1 = await _repo.UpdateAsync(team);
        await DbContext.SaveChangesAsync();
        // Assert
        result1.Name.ShouldBe("First Update");

        team.Update(
            Name.Create("Second Update"),
            DescriptionNullable.Create("Second"));

        // Act
        var result2 = await _repo.UpdateAsync(team);
        await DbContext.SaveChangesAsync();

        // Assert
        result2.Name.ShouldBe("Second Update");
        
        // Verify final state
        var finalTeam = await DbContext.Teams.FindAsync(team.Id);
        finalTeam!.Name.ShouldBe("Second Update");
        finalTeam.Description.ShouldBe("Second");
    }

    #endregion

}//Cls
