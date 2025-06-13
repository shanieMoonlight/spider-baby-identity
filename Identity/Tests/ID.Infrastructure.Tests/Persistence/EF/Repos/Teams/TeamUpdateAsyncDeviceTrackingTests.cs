using ClArch.ValueObjects;
using ID.Domain.Entities.Devices;
using ID.Domain.Entities.Teams;

namespace ID.Infrastructure.Tests.Persistence.EF.Repos.Teams;

/// <summary>
/// Tests the TeamRepo's custom UpdateAsync override that handles device tracking.
/// This focuses ONLY on the AddNewDevicesToDbAsync logic - ensuring new devices get added to DB.
/// TeamRepo is the only repo with this special UpdateAsync override.
/// </summary>
public class TeamUpdateAsyncDeviceTrackingTests : RepoTestBase, IAsyncLifetime
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
    // DEVICE TRACKING TESTS
    //=================================//

    #region Device Tracking Tests

    [Fact]
    public async Task UpdateAsync_WithNewDevicesInSubscription_ShouldAddNewDevicesToDatabase()
    {
        // Arrange - Create team with subscription but no devices initially
        var teamId = Guid.NewGuid();
        var subscription = SubscriptionDataFactory.Create(teamId: teamId);
        var team = TeamDataFactory.Create(id: teamId, teamType: TeamType.Customer);
        
        // Add subscription to team (using proper domain method would require validation token)
        // For testing, we'll use the factory to set up the relationship
        await DbContext.Teams.AddAsync(team);
        await DbContext.Set<TeamSubscription>().AddAsync(subscription);
        await DbContext.SaveChangesAsync();

        // Now add devices to the subscription 
        var device1 = DeviceDataFactory.Create(subscriptionId: subscription.Id);
        var device2 = DeviceDataFactory.Create(subscriptionId: subscription.Id);

        // Manually add devices to subscription's device collection for testing
        subscription.AddDevice(
            Name.Create(device1.Name),
            DescriptionNullable.Create(device1.Description),
            UniqueId.Create(device1.UniqueId));
        
        subscription.AddDevice(
            Name.Create(device2.Name),
            DescriptionNullable.Create(device2.Description),
            UniqueId.Create(device2.UniqueId));

        // Act - Update team (this should trigger AddNewDevicesToDbAsync)
        await _repo.UpdateAsync(team);
        await DbContext.SaveChangesAsync();

        // Assert - New devices should be in database
        var devicesInDb = await DbContext.Set<TeamDevice>()
            .Where(d => d.SubscriptionId == subscription.Id)
            .ToListAsync();

        devicesInDb.Count.ShouldBe(2);
        devicesInDb.ShouldContain(d => d.Name == device1.Name);
        devicesInDb.ShouldContain(d => d.Name == device2.Name);
    }

    //-----------------------//

    [Fact]
    public async Task UpdateAsync_WithExistingDevices_ShouldNotDuplicateDevices()
    {
        // Arrange - Create team with subscription and existing device
        var teamId = Guid.NewGuid();
        var subscription = SubscriptionDataFactory.Create(teamId: teamId);
        var team = TeamDataFactory.Create(id: teamId, teamType: TeamType.Customer);
        var existingDevice = DeviceDataFactory.Create(subscriptionId: subscription.Id);

        // Set up initial state with existing device
        await DbContext.Teams.AddAsync(team);
        await DbContext.Set<TeamSubscription>().AddAsync(subscription);
        await DbContext.Set<TeamDevice>().AddAsync(existingDevice);
        await DbContext.SaveChangesAsync();

        // Add the existing device to subscription (simulate it's already there)
        subscription.AddDevice(
            Name.Create(existingDevice.Name),
            DescriptionNullable.Create(existingDevice.Description),
            UniqueId.Create(existingDevice.UniqueId));

        // Act - Update team 
        await _repo.UpdateAsync(team);
        await DbContext.SaveChangesAsync();

        // Assert - Should still only have one device (no duplicates)
        var devicesInDb = await DbContext.Set<TeamDevice>()
            .Where(d => d.SubscriptionId == subscription.Id)
            .ToListAsync();

        devicesInDb.Count.ShouldBe(1);
        devicesInDb.First().Id.ShouldBe(existingDevice.Id);
    }

    //-----------------------//

    [Fact]
    public async Task UpdateAsync_WithMixOfNewAndExistingDevices_ShouldOnlyAddNewDevices()
    {
        // Arrange - Create team with subscription and one existing device
        var teamId = Guid.NewGuid();
        var subscription = SubscriptionDataFactory.Create(teamId: teamId);
        var team = TeamDataFactory.Create(id: teamId, teamType: TeamType.Customer);
        var existingDevice = DeviceDataFactory.Create(subscriptionId: subscription.Id);
        var newDevice = DeviceDataFactory.Create(subscriptionId: subscription.Id);

        // Set up initial state with existing device
        await DbContext.Teams.AddAsync(team);
        await DbContext.Set<TeamSubscription>().AddAsync(subscription);
        await DbContext.Set<TeamDevice>().AddAsync(existingDevice);
        await DbContext.SaveChangesAsync();

        // Add both existing and new device to subscription
        subscription.AddDevice(
            Name.Create(existingDevice.Name),
            DescriptionNullable.Create(existingDevice.Description),
            UniqueId.Create(existingDevice.UniqueId));
            
        subscription.AddDevice(
            Name.Create(newDevice.Name),
            DescriptionNullable.Create(newDevice.Description),
            UniqueId.Create(newDevice.UniqueId));

        // Act - Update team
        await _repo.UpdateAsync(team);
        await DbContext.SaveChangesAsync();

        // Assert - Should have both devices (1 existing + 1 new)
        var devicesInDb = await DbContext.Set<TeamDevice>()
            .Where(d => d.SubscriptionId == subscription.Id)
            .ToListAsync();

        devicesInDb.Count.ShouldBe(2);
        devicesInDb.ShouldContain(d => d.Id == existingDevice.Id);
        devicesInDb.ShouldContain(d => d.Name == newDevice.Name && d.Id != existingDevice.Id);
    }

    //-----------------------//

    [Fact]
    public async Task UpdateAsync_WithMultipleSubscriptionsAndDevices_ShouldHandleAllCorrectly()
    {
        // Arrange - Create team with multiple subscriptions
        var teamId = Guid.NewGuid();
        var subscription1 = SubscriptionDataFactory.Create(teamId: teamId);
        var subscription2 = SubscriptionDataFactory.Create(teamId: teamId);
        var team = TeamDataFactory.Create(id: teamId, teamType: TeamType.Customer);
        
        var device1ForSub1 = DeviceDataFactory.Create(subscriptionId: subscription1.Id);
        var device2ForSub2 = DeviceDataFactory.Create(subscriptionId: subscription2.Id);

        await DbContext.Teams.AddAsync(team);
        await DbContext.Set<TeamSubscription>().AddRangeAsync(subscription1, subscription2);
        await DbContext.SaveChangesAsync();

        // Add devices to both subscriptions
        subscription1.AddDevice(
            Name.Create(device1ForSub1.Name),
            DescriptionNullable.Create(device1ForSub1.Description),
            UniqueId.Create(device1ForSub1.UniqueId));
            
        subscription2.AddDevice(
            Name.Create(device2ForSub2.Name),
            DescriptionNullable.Create(device2ForSub2.Description),
            UniqueId.Create(device2ForSub2.UniqueId));

        // Act - Update team
        await _repo.UpdateAsync(team);
        await DbContext.SaveChangesAsync();

        // Assert - Both devices should be added to their respective subscriptions
        var devicesForSub1 = await DbContext.Set<TeamDevice>()
            .Where(d => d.SubscriptionId == subscription1.Id)
            .ToListAsync();
            
        var devicesForSub2 = await DbContext.Set<TeamDevice>()
            .Where(d => d.SubscriptionId == subscription2.Id)
            .ToListAsync();

        devicesForSub1.Count.ShouldBe(1);
        devicesForSub1.First().Name.ShouldBe(device1ForSub1.Name);
        
        devicesForSub2.Count.ShouldBe(1);
        devicesForSub2.First().Name.ShouldBe(device2ForSub2.Name);
    }

    //-----------------------//

    [Fact]
    public async Task UpdateAsync_WithNoDevicesInSubscription_ShouldWorkWithoutErrors()
    {
        // Arrange - Create team with subscription but no devices
        var subscription = SubscriptionDataFactory.Create();
        var team = TeamDataFactory.Create(teamType: TeamType.Customer);

        await DbContext.Teams.AddAsync(team);
        await DbContext.Set<TeamSubscription>().AddAsync(subscription);
        await DbContext.SaveChangesAsync();

        // Act - Update team (no devices to process)
        var result = await _repo.UpdateAsync(team);
        await DbContext.SaveChangesAsync();

        // Assert - Should work without errors
        result.ShouldNotBeNull();
        
        var devicesInDb = await DbContext.Set<TeamDevice>()
            .Where(d => d.SubscriptionId == subscription.Id)
            .ToListAsync();
            
        devicesInDb.Count.ShouldBe(0);
    }

    //-----------------------//

    [Fact]
    public async Task UpdateAsync_WithNoSubscriptions_ShouldWorkWithoutErrors()
    {
        // Arrange - Create team with no subscriptions
        var team = TeamDataFactory.Create(teamType: TeamType.Customer);

        await DbContext.Teams.AddAsync(team);
        await DbContext.SaveChangesAsync();

        // Act - Update team (no subscriptions to process)
        var result = await _repo.UpdateAsync(team);
        await DbContext.SaveChangesAsync();

        // Assert - Should work without errors
        result.ShouldNotBeNull();
    }

    #endregion

}//Cls
