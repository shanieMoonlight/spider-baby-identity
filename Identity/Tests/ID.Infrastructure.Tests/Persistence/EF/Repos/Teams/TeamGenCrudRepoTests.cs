//using ID.Domain.Entities.Teams;
//using ID.Infrastructure.Persistance.EF.Repos;
//using ID.Tests.Data.Factories;
//using Shouldly;

//namespace ID.Infrastructure.Tests.Persistence.EF.Repos.Teams;

//public class TeamGenCrudRepoTests : RepoTestBase, IAsyncLifetime
//{
//    private TeamRepo _repo = null!;

//    //-----------------------------//

//    public async Task InitializeAsync()
//    {
//        _repo = new TeamRepo(DbContext);
//        await Task.CompletedTask;
//    }

//    public Task DisposeAsync() => Task.CompletedTask;

//    //=================================//
//    // ADD TESTS
//    //=================================//

//    #region Add Tests

//    [Fact]
//    public async Task AddAsync_WithValidTeam_ShouldAddTeam()
//    {
//        // Arrange
//        var team = TeamDataFactory.Create(teamType: TeamType.Customer);

//        // Act
//        var result = await _repo.AddAsync(team);

//        // Assert
//        result.ShouldNotBeNull();
//        result.Id.ShouldBe(team.Id);
//        result.Name.ShouldBe(team.Name);
//        result.TeamType.ShouldBe(TeamType.Customer);
//    }

//    [Fact]
//    public async Task AddAsync_WithTeamWithMembers_ShouldAddTeamAndMembers()
//    {
//        // Arrange
//        var leader = AppUserDataFactory.Create();
//        var member = AppUserDataFactory.Create();
//        var team = TeamDataFactory.Create(
//            teamType: TeamType.Customer,
//            leader: leader,
//            members: [leader, member]);

//        // Act
//        var result = await _repo.AddAsync(team);

//        // Assert
//        result.ShouldNotBeNull();
//        result.Members.ShouldContain(m => m.Id == leader.Id);
//        result.Members.ShouldContain(m => m.Id == member.Id);
//        result.LeaderId.ShouldBe(leader.Id);
//    }

//    [Fact]
//    public async Task AddAsync_WithTeamWithSubscriptions_ShouldAddTeamAndSubscriptions()
//    {
//        // Arrange
//        var subscription = SubscriptionDataFactory.Create();
//        var team = TeamDataFactory.Create(teamType: TeamType.Customer, subscriptions: [subscription]);

//        // Act
//        var result = await _repo.AddAsync(team);

//        // Assert
//        result.ShouldNotBeNull();
//        result.Subscriptions.ShouldContain(s => s.Id == subscription.Id);
//    }

//    [Fact]
//    public async Task AddAsync_WithSuperTeam_ShouldAddSuperTeam()
//    {
//        // Arrange
//        var team = TeamDataFactory.Create(teamType: TeamType.Super);

//        // Act
//        var result = await _repo.AddAsync(team);

//        // Assert
//        result.ShouldNotBeNull();
//        result.TeamType.ShouldBe(TeamType.Super);
//    }

//    [Fact]
//    public async Task AddAsync_WithMaintenanceTeam_ShouldAddMaintenanceTeam()
//    {
//        // Arrange
//        var team = TeamDataFactory.Create(teamType: TeamType.Maintenance);

//        // Act
//        var result = await _repo.AddAsync(team);

//        // Assert
//        result.ShouldNotBeNull();
//        result.TeamType.ShouldBe(TeamType.Maintenance);
//    }

//    #endregion

//    //=================================//
//    // GET TESTS
//    //=================================//

//    #region Get Tests

//    [Fact]
//    public async Task GetByIdAsync_WithExistingId_ShouldReturnTeam()
//    {
//        // Arrange
//        var team = TeamDataFactory.Create(teamType: TeamType.Customer);
//        await _repo.AddAsync(team);
//        await DbContext.SaveChangesAsync();

//        // Act
//        var result = await _repo.GetByIdAsync(team.Id);

//        // Assert
//        result.ShouldNotBeNull();
//        result.Id.ShouldBe(team.Id);
//        result.Name.ShouldBe(team.Name);
//    }

//    [Fact]
//    public async Task GetByIdAsync_WithNonExistingId_ShouldReturnNull()
//    {
//        // Arrange
//        var nonExistingId = Guid.NewGuid();

//        // Act
//        var result = await _repo.GetByIdAsync(nonExistingId);

//        // Assert
//        result.ShouldBeNull();
//    }

//    [Fact]
//    public async Task ListAllAsync_WithMultipleTeams_ShouldReturnAllTeams()
//    {
//        // Arrange
//        var teams = TeamDataFactory.CreateMany(5);
//        foreach (var team in teams)
//        {
//            await _repo.AddAsync(team);
//        }
//        await DbContext.SaveChangesAsync();

//        // Act
//        var result = await _repo.ListAllAsync();

//        // Assert
//        result.ShouldNotBeNull();
//        result.Count.ShouldBeGreaterThanOrEqualTo(5);
//        foreach (var team in teams)
//        {
//            result.ShouldContain(t => t.Id == team.Id);
//        }
//    }

//    [Fact]
//    public async Task CountAsync_WithMultipleTeams_ShouldReturnCorrectCount()
//    {
//        // Arrange
//        var teams = TeamDataFactory.CreateMany(3);
//        foreach (var team in teams)
//        {
//            await _repo.AddAsync(team);
//        }
//        await DbContext.SaveChangesAsync();

//        // Act
//        var result = await _repo.CountAsync();

//        // Assert
//        result.ShouldBeGreaterThanOrEqualTo(3);
//    }

//    [Fact]
//    public async Task FirstOrDefaultAsync_WithExistingTeam_ShouldReturnTeam()
//    {
//        // Arrange
//        var team = TeamDataFactory.Create(
//            name: "UniqueTeamName",
//            teamType: TeamType.Customer);
//        await _repo.AddAsync(team);
//        await DbContext.SaveChangesAsync();

//        // Act
//        var result = await _repo.FirstOrDefaultAsync(t => t.Name == "UniqueTeamName");

//        // Assert
//        result.ShouldNotBeNull();
//        result.Id.ShouldBe(team.Id);
//        result.Name.ShouldBe("UniqueTeamName");
//    }

//    [Fact]
//    public async Task FirstOrDefaultAsync_WithNonExistingTeam_ShouldReturnNull()
//    {
//        // Act
//        var result = await _repo.FirstOrDefaultAsync(t => t.Name == "NonExistentTeam");

//        // Assert
//        result.ShouldBeNull();
//    }

//    #endregion

//    //=================================//
//    // UPDATE TESTS
//    //=================================//

//    #region Update Tests

//    [Fact]
//    public async Task UpdateAsync_WithValidTeam_ShouldUpdateTeam()
//    {
//        // Arrange
//        var team = TeamDataFactory.Create(teamType: TeamType.Customer);
//        await _repo.AddAsync(team);
//        await DbContext.SaveChangesAsync();

//        team.Update(
//            ClArch.ValueObjects.Name.Create("Updated Name"),
//            ClArch.ValueObjects.DescriptionNullable.Create("Updated Description"));

//        // Act
//        var result = await _repo.UpdateAsync(team);
//        await DbContext.SaveChangesAsync();

//        // Assert
//        result.ShouldNotBeNull();
//        result.Name.ShouldBe("Updated Name");
//        result.Description.ShouldBe("Updated Description");
//    }

//    [Fact]
//    public async Task UpdateAsync_WithTeamPositionChanges_ShouldUpdatePositions()
//    {
//        // Arrange
//        var team = TeamDataFactory.Create(
//            teamType: TeamType.Customer,
//            minPosition: 1,
//            maxPosition: 5);
//        await _repo.AddAsync(team);
//        await DbContext.SaveChangesAsync();

//        // Update position range
//        team.UpdatePositionRange(
//            ID.Domain.Entities.Teams.Validators.TeamValidators.PositionRangeUpdate.Validate(
//                team, new ID.Domain.Entities.Teams.ValueObjects.PositionRange(2, 8)).Value);

//        // Act
//        var result = await _repo.UpdateAsync(team);
//        await DbContext.SaveChangesAsync();

//        // Assert
//        result.ShouldNotBeNull();
//        result.MinPosition.ShouldBe(2);
//        result.MaxPosition.ShouldBe(8);
//    }

//    [Fact]
//    public async Task UpdateAsync_WithLeaderChange_ShouldUpdateLeader()
//    {
//        // Arrange
//        var originalLeader = AppUserDataFactory.Create();
//        var newLeader = AppUserDataFactory.Create();
//        var team = TeamDataFactory.Create(
//            teamType: TeamType.Customer,
//            leader: originalLeader,
//            members: [originalLeader, newLeader]);

//        await _repo.AddAsync(team);
//        await DbContext.AppUsers.AddRangeAsync(originalLeader, newLeader);
//        await DbContext.SaveChangesAsync();

//        // Change leader
//        var leaderToken = ID.Domain.Entities.Teams.Validators.TeamValidators.LeaderUpdate.Validate(team, newLeader);
//        team.SetLeader(leaderToken.Value);

//        // Act
//        var result = await _repo.UpdateAsync(team);
//        await DbContext.SaveChangesAsync();

//        // Assert
//        result.ShouldNotBeNull();
//        result.LeaderId.ShouldBe(newLeader.Id);
//        result.Leader?.Id.ShouldBe(newLeader.Id);
//    }

//    #endregion

//    //=================================//
//    // DELETE TESTS
//    //=================================//

//    #region Delete Tests

//    [Fact]
//    public async Task DeleteAsync_WithValidCustomerTeam_ShouldDeleteTeam()
//    {
//        // Arrange
//        var team = TeamDataFactory.Create(teamType: TeamType.Customer);
//        await _repo.AddAsync(team);
//        await DbContext.SaveChangesAsync();

//        // Act
//        await _repo.DeleteAsync(team);
//        await DbContext.SaveChangesAsync();

//        // Assert
//        var deletedTeam = await _repo.GetByIdAsync(team.Id);
//        deletedTeam.ShouldBeNull();
//    }

//    [Fact]
//    public async Task DeleteAsync_WithSuperTeam_ShouldNotDelete()
//    {
//        // Arrange
//        var team = TeamDataFactory.Create(teamType: TeamType.Super);
//        await _repo.AddAsync(team);
//        await DbContext.SaveChangesAsync();

//        // Act & Assert
//        await Should.ThrowAsync<Exception>(async () =>
//        {
//            await _repo.DeleteAsync(team);
//            await DbContext.SaveChangesAsync();
//        });
//    }

//    [Fact]
//    public async Task DeleteAsync_WithMaintenanceTeam_ShouldNotDelete()
//    {
//        // Arrange
//        var team = TeamDataFactory.Create(teamType: TeamType.Maintenance);
//        await _repo.AddAsync(team);
//        await DbContext.SaveChangesAsync();

//        // Act & Assert
//        await Should.ThrowAsync<Exception>(async () =>
//        {
//            await _repo.DeleteAsync(team);
//            await DbContext.SaveChangesAsync();
//        });
//    }

//    [Fact]
//    public async Task DeleteAsync_WithTeamWithMembers_ShouldNotDelete()
//    {
//        // Arrange
//        var leader = AppUserDataFactory.Create();
//        var member = AppUserDataFactory.Create();
//        var team = TeamDataFactory.Create(
//            teamType: TeamType.Customer,
//            leader: leader,
//            members: [leader, member]);

//        await _repo.AddAsync(team);
//        await DbContext.AppUsers.AddRangeAsync(leader, member);
//        await DbContext.SaveChangesAsync();

//        // Act & Assert
//        await Should.ThrowAsync<Exception>(async () =>
//        {
//            await _repo.DeleteAsync(team);
//            await DbContext.SaveChangesAsync();
//        });
//    }

//    [Fact]
//    public async Task DeleteByIdAsync_WithValidId_ShouldDeleteTeam()
//    {
//        // Arrange
//        var team = TeamDataFactory.Create(teamType: TeamType.Customer);
//        await _repo.AddAsync(team);
//        await DbContext.SaveChangesAsync();

//        // Act
//        await _repo.DeleteByIdAsync(team.Id);
//        await DbContext.SaveChangesAsync();

//        // Assert
//        var deletedTeam = await _repo.GetByIdAsync(team.Id);
//        deletedTeam.ShouldBeNull();
//    }

//    [Fact]
//    public async Task DeleteByIdAsync_WithNonExistingId_ShouldNotThrow()
//    {
//        // Arrange
//        var nonExistingId = Guid.NewGuid();

//        // Act & Assert
//        await Should.NotThrowAsync(async () =>
//            await _repo.DeleteByIdAsync(nonExistingId));
//    }

//    #endregion

//    //=================================//
//    // PERSISTENCE TESTS
//    //=================================//

//    #region Persistence Tests

//    [Fact]
//    public async Task AddAsync_WithComplexTeamStructure_ShouldPersistCorrectly()
//    {
//        // Arrange
//        var leader = AppUserDataFactory.Create();
//        var member1 = AppUserDataFactory.Create();
//        var member2 = AppUserDataFactory.Create();
//        var subscription = SubscriptionDataFactory.Create();
//        var device = DeviceDataFactory.Create(subscriptionId: subscription.Id);
        
//        subscription.AddDevice(device);
        
//        var team = TeamDataFactory.Create(
//            teamType: TeamType.Customer,
//            leader: leader,
//            members: [leader, member1, member2]);
        
//        team.AddSubscription(subscription);

//        // Act
//        var result = await _repo.AddAsync(team);
//        await DbContext.SaveChangesAsync();

//        // Assert
//        var savedTeam = await DbContext.Teams
//            .Include(t => t.Members)
//            .Include(t => t.Subscriptions)
//                .ThenInclude(s => s.Devices)
//            .FirstAsync(t => t.Id == team.Id);

//        savedTeam.ShouldNotBeNull();
//        savedTeam.Members.Count.ShouldBe(3);
//        savedTeam.Subscriptions.Count.ShouldBe(1);
//        savedTeam.Subscriptions.First().Devices.Count.ShouldBe(1);
//    }

//    [Fact]
//    public async Task UpdateAsync_WithMembershipChanges_ShouldUpdateCorrectly()
//    {
//        // Arrange
//        var leader = AppUserDataFactory.Create();
//        var originalMember = AppUserDataFactory.Create();
//        var team = TeamDataFactory.Create(
//            teamType: TeamType.Customer,
//            leader: leader,
//            members: [leader, originalMember]);

//        await _repo.AddAsync(team);
//        await DbContext.AppUsers.AddRangeAsync(leader, originalMember);
//        await DbContext.SaveChangesAsync();

//        // Add new member
//        var newMember = AppUserDataFactory.Create();
//        var addToken = ID.Domain.Entities.Teams.Validators.TeamValidators.MemberAddition.Validate(team, newMember);
//        team.AddMember(addToken.Value);

//        // Act
//        var result = await _repo.UpdateAsync(team);
//        await DbContext.SaveChangesAsync();

//        // Assert
//        var updatedTeam = await DbContext.Teams
//            .Include(t => t.Members)
//            .FirstAsync(t => t.Id == team.Id);

//        updatedTeam.Members.Count.ShouldBe(3);
//        updatedTeam.Members.ShouldContain(m => m.Id == newMember.Id);
//    }

//    #endregion

//    //=================================//
//    // CONCURRENCY TESTS
//    //=================================//

//    #region Concurrency Tests

//    [Fact]
//    public async Task UpdateAsync_WithConcurrentModification_ShouldHandleCorrectly()
//    {
//        // Arrange
//        var team = TeamDataFactory.Create(teamType: TeamType.Customer);
//        await _repo.AddAsync(team);
//        await DbContext.SaveChangesAsync();

//        // Get two instances of the same team
//        var team1 = await _repo.GetByIdAsync(team.Id);
//        var team2 = await _repo.GetByIdAsync(team.Id);

//        // Modify both instances
//        team1!.Update(
//            ClArch.ValueObjects.Name.Create("Update 1"),
//            ClArch.ValueObjects.DescriptionNullable.Create("Description 1"));
        
//        team2!.Update(
//            ClArch.ValueObjects.Name.Create("Update 2"),
//            ClArch.ValueObjects.DescriptionNullable.Create("Description 2"));

//        // Act
//        await _repo.UpdateAsync(team1);
//        await DbContext.SaveChangesAsync();

//        // Second update should work (EF Core behavior)
//        await _repo.UpdateAsync(team2);
//        await DbContext.SaveChangesAsync();

//        // Assert
//        var finalTeam = await _repo.GetByIdAsync(team.Id);
//        finalTeam.ShouldNotBeNull();
//        finalTeam.Name.ShouldBe("Update 2"); // Last update wins
//    }

//    #endregion

//}//Cls
