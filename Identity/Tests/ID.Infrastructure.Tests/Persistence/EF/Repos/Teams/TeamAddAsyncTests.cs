using ID.Domain.Entities.Teams;

namespace ID.Infrastructure.Tests.Persistence.EF.Repos.Teams;

public class TeamAddAsyncTests : RepoTestBase, IAsyncLifetime
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
    // ADD ASYNC TESTS
    //=================================//

    #region Basic Add Tests

    [Fact]
    public async Task AddAsync_WithValidCustomerTeam_ShouldAddTeam()
    {
        // Arrange
        var team = TeamDataFactory.Create(
            name: "Test Customer Team",
            description: "A test customer team",
            teamType: TeamType.Customer);

        // Act
        var result = await _repo.AddAsync(team);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(team.Id);
        result.Name.ShouldBe("Test Customer Team");
        result.Description.ShouldBe("A test customer team");
        result.TeamType.ShouldBe(TeamType.Customer);
    }

    [Fact]
    public async Task AddAsync_WithValidMaintenanceTeam_ShouldAddTeam()
    {
        // Arrange
        var team = TeamDataFactory.Create(
            name: "Test Maintenance Team",
            description: "A test maintenance team",
            teamType: TeamType.Maintenance);

        // Act
        var result = await _repo.AddAsync(team);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(team.Id);
        result.Name.ShouldBe("Test Maintenance Team");
        result.TeamType.ShouldBe(TeamType.Maintenance);
    }

    [Fact]
    public async Task AddAsync_WithValidSuperTeam_ShouldAddTeam()
    {
        // Arrange
        var team = TeamDataFactory.Create(
            name: "Test Super Team",
            description: "A test super team",
            teamType: TeamType.Super);

        // Act
        var result = await _repo.AddAsync(team);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(team.Id);
        result.Name.ShouldBe("Test Super Team");
        result.TeamType.ShouldBe(TeamType.Super);
    }



    #endregion

    //=================================//
    // ADD WITH RELATIONSHIPS TESTS
    //=================================//

    #region Relationship Tests

    [Fact]
    public async Task AddAsync_WithLeader_ShouldAddTeamWithLeader()
    {
        // Arrange
        var leader = AppUserDataFactory.Create();
        var team = TeamDataFactory.Create(
            name: "Team with Leader",
            teamType: TeamType.Customer,
            leader: leader,
            members: [leader]);

        // Act
        var result = await _repo.AddAsync(team);

        // Assert
        result.ShouldNotBeNull();
        result.LeaderId.ShouldBe(leader.Id);
        result.Leader.ShouldNotBeNull();
        result.Leader!.Id.ShouldBe(leader.Id);
        result.Members.ShouldContain(m => m.Id == leader.Id);
    }

    [Fact]
    public async Task AddAsync_WithMultipleMembers_ShouldAddTeamWithAllMembers()
    {
        // Arrange
        var leader = AppUserDataFactory.Create();
        var member1 = AppUserDataFactory.Create();
        var member2 = AppUserDataFactory.Create();
        var members = new HashSet<ID.Domain.Entities.AppUsers.AppUser> { leader, member1, member2 };
        
        var team = TeamDataFactory.Create(
            name: "Team with Multiple Members",
            teamType: TeamType.Customer,
            leader: leader,
            members: members);

        // Act
        var result = await _repo.AddAsync(team);

        // Assert
        result.ShouldNotBeNull();
        result.Members.Count.ShouldBe(3);
        result.Members.ShouldContain(m => m.Id == leader.Id);
        result.Members.ShouldContain(m => m.Id == member1.Id);
        result.Members.ShouldContain(m => m.Id == member2.Id);
    }

    [Fact] 
    public async Task AddAsync_WithCustomCapacity_ShouldPreserveCapacity()
    {
        // Arrange
        var team = TeamDataFactory.Create(
            name: "Team with Custom Capacity",
            teamType: TeamType.Customer,
            capacity: 50);

        // Act
        var result = await _repo.AddAsync(team);

        // Assert
        result.ShouldNotBeNull();
        result.Capacity.ShouldBe(50);
    }

    [Fact]
    public async Task AddAsync_WithCustomPositionRange_ShouldPreservePositionRange()
    {
        // Arrange
        var team = TeamDataFactory.Create(
            name: "Team with Custom Positions",
            teamType: TeamType.Customer,
            minPosition: 2,
            maxPosition: 8);

        // Act
        var result = await _repo.AddAsync(team);

        // Assert
        result.ShouldNotBeNull();
        result.MinPosition.ShouldBe(2);
        result.MaxPosition.ShouldBe(8);
    }

    #endregion

    //=================================//
    // PERSISTENCE VERIFICATION TESTS
    //=================================//

    #region Persistence Tests

    [Fact]
    public async Task AddAsync_ShouldPersistAfterSaveChanges()
    {
        // Arrange
        var team = TeamDataFactory.Create(
            name: "Persistence Test Team",
            teamType: TeamType.Customer);

        // Act
        var addedTeam = await _repo.AddAsync(team);
        await DbContext.SaveChangesAsync();

        // Verify persistence by retrieving from database
        var persistedTeam = await DbContext.Teams.FindAsync(team.Id);

        // Assert
        addedTeam.ShouldNotBeNull();
        persistedTeam.ShouldNotBeNull();
        persistedTeam.Id.ShouldBe(team.Id);
        persistedTeam.Name.ShouldBe("Persistence Test Team");
        persistedTeam.TeamType.ShouldBe(TeamType.Customer);
    }

    [Fact]
    public async Task AddAsync_MultipleTeams_ShouldAddAllTeams()
    {
        // Arrange
        var team1 = TeamDataFactory.Create(name: "Team 1", teamType: TeamType.Customer);
        var team2 = TeamDataFactory.Create(name: "Team 2", teamType: TeamType.Maintenance);
        var team3 = TeamDataFactory.Create(name: "Team 3", teamType: TeamType.Super);

        // Act
        var result1 = await _repo.AddAsync(team1);
        var result2 = await _repo.AddAsync(team2);
        var result3 = await _repo.AddAsync(team3);
        await DbContext.SaveChangesAsync();

        // Assert
        result1.Name.ShouldBe("Team 1");
        result2.Name.ShouldBe("Team 2");
        result3.Name.ShouldBe("Team 3");

        // Verify all are persisted
        var allTeams = DbContext.Teams.ToList();
        allTeams.ShouldContain(t => t.Id == team1.Id);
        allTeams.ShouldContain(t => t.Id == team2.Id);        allTeams.ShouldContain(t => t.Id == team3.Id);
    }

    #endregion

    //=================================//
    // EDGE CASE TESTS
    //=================================//

    #region Edge Cases

    [Fact]
    public async Task AddAsync_WithNullDescription_ShouldHandleGracefully()
    {
        // Arrange
        var team = TeamDataFactory.Create(
            name: "Team with Null Description",
            description: null,
            teamType: TeamType.Customer);

        // Act
        var result = await _repo.AddAsync(team);

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Team with Null Description");
        result.Description.ShouldBeNull();
    }

    [Fact]
    public async Task AddAsync_WithLongName_ShouldPreserveLongName()
    {
        // Arrange
        var longName = new string('A', 100); // 100 character name
        var team = TeamDataFactory.Create(
            name: longName,
            teamType: TeamType.Customer);

        // Act
        var result = await _repo.AddAsync(team);

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe(longName);
        result.Name.Length.ShouldBe(100);
    }

    [Fact]
    public async Task AddAsync_WithSpecialCharactersInName_ShouldPreserveCharacters()
    {
        // Arrange
        var specialName = "Team-With_Special@Characters#2024!";
        var team = TeamDataFactory.Create(
            name: specialName,
            teamType: TeamType.Customer);

        // Act
        var result = await _repo.AddAsync(team);

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe(specialName);
    }

    #endregion

    //=================================//
    // ERROR HANDLING TESTS
    //=================================//

    #region Error Handling

    [Fact]
    public async Task AddAsync_WithNullTeam_ShouldThrowException()
    {
        // Arrange
        Team nullTeam = null!;

        // Act & Assert
        await Should.ThrowAsync<ArgumentNullException>(async () =>
            await _repo.AddAsync(nullTeam));
    }

    [Fact]
    public async Task AddAsync_WithDuplicateId_ShouldHandleCorrectly()
    {
        // Arrange
        var team1 = TeamDataFactory.Create(name: "First Team", teamType: TeamType.Customer);
        var team2 = TeamDataFactory.Create(
            id: team1.Id, // Same ID
            name: "Second Team", 
            teamType: TeamType.Maintenance);

        // Act
        await _repo.AddAsync(team1);
        await DbContext.SaveChangesAsync();

        // Assert - Should throw when trying to add duplicate ID
        await Should.ThrowAsync<Exception>(async () =>
        {
            await _repo.AddAsync(team2);
            await DbContext.SaveChangesAsync();
        });
    }

    #endregion

}//Cls
