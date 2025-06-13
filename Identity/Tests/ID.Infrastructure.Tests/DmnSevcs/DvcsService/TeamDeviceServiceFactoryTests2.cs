using ID.Domain.Entities.Teams;
using ID.Infrastructure.DomainServices.Teams.Dvcs;
using ID.Infrastructure.Persistance.Abstractions.Repos;
using Moq;

namespace ID.Infrastructure.Tests.DmnSevcs.DvcsService;

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
public class TeamDeviceServiceFactoryTests
{
    private readonly Mock<IIdUnitOfWork> _mockUow;
    private readonly TeamDeviceServiceFactory _factory;

    public TeamDeviceServiceFactoryTests()
    {
        _mockUow = new Mock<IIdUnitOfWork>();
        _factory = new TeamDeviceServiceFactory(_mockUow.Object);
    }

    //------------------------------------//  

    [Fact]
    public async Task GetServiceAsync_WithValidTeamIdAndSubscriptionId_ReturnsService()
    {
        // Arrange  
        var teamId = Guid.NewGuid();

        var sub1 = SubscriptionDataFactory.Create(teamId: teamId);
        var sub2 = SubscriptionDataFactory.Create(teamId: teamId);
        var team = TeamDataFactory.Create(id: teamId, subscriptions: [sub1, sub2]);

        _mockUow.Setup(u => u.TeamRepo.FirstOrDefaultAsync(It.IsAny<TeamByIdWithSubscriptionsSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(team);

        // Act  
        var result = await _factory.GetServiceAsync(teamId, sub1.Id);

        // Assert  
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.ShouldBeOfType<TeamDeviceService>();
    }

    //------------------------------------//  

    [Fact]
    public async Task GetServiceAsync_WithInvalidTeamId_ReturnsNotFoundResult()
    {
        // Arrange  
        var teamId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();

        _mockUow.Setup(u => u.TeamRepo.FirstOrDefaultAsync(It.IsAny<TeamByIdWithSubscriptionsSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Team)null);

        // Act  
        var result = await _factory.GetServiceAsync(teamId, subscriptionId);

        // Assert  
        result.Succeeded.ShouldBeFalse();
        result.NotFound.ShouldBeTrue();
    }

    //------------------------------------//  

    [Fact]
    public async Task GetServiceAsync_WithNullSubscriptionId_ReturnsNotFoundResult()
    {
        // Arrange  
        var teamId = Guid.NewGuid();


        _mockUow.Setup(u => u.TeamRepo.FirstOrDefaultAsync(It.IsAny<TeamByIdWithSubscriptionsSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(TeamDataFactory.Create()); //Returns a valid team but should nver be called when the subscriptionId is null

        // Act  
        var result = await _factory.GetServiceAsync(teamId, null);

        // Assert  
        result.Succeeded.ShouldBeFalse();
        result.NotFound.ShouldBeTrue();
    }

    //------------------------------------//  

    [Fact]
    public async Task GetServiceAsync_WithInvalidSubscriptionId_ReturnsNotFoundResult()
    {
        // Arrange  
        var teamId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();
        var team = TeamDataFactory.Create(id: teamId);

        _mockUow.Setup(u => u.TeamRepo.FirstOrDefaultAsync(It.IsAny<TeamByIdWithSubscriptionsSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(team);

        // Act  
        var result = await _factory.GetServiceAsync(teamId, subscriptionId);

        // Assert  
        result.Succeeded.ShouldBeFalse();
        result.NotFound.ShouldBeTrue();
    }

    //------------------------------------//  

    [Fact]
    public async Task GetServiceAsync_WithValidTeamAndSubscriptionId_ReturnsService()
    {
        // Arrange  
        var teamId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();
        var sub = SubscriptionDataFactory.Create(id: subscriptionId, teamId: teamId);
        var team = TeamDataFactory.Create(id: teamId, subscriptions: [sub]);

        // Act  
        var result = await _factory.GetServiceAsync(team, subscriptionId);

        // Assert  
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.ShouldBeOfType<TeamDeviceService>();
    }

    //------------------------------------//  

    [Fact]
    public async Task GetServiceAsync_WithNullSubscriptionIdInTeam_ReturnsNotFoundResult()
    {
        // Arrange  
        var team = TeamDataFactory.Create();

        // Act  
        var result = await _factory.GetServiceAsync(team, null);

        // Assert  
        result.Succeeded.ShouldBeFalse();
        result.NotFound.ShouldBeTrue();
    }

    //------------------------------------//  

    [Fact]
    public async Task GetServiceAsync_WithInvalidSubscriptionIdInTeam_ReturnsNotFoundResult()
    {
        // Arrange  
        var team = TeamDataFactory.Create();
        var subscriptionId = Guid.NewGuid();

        // Act  
        var result = await _factory.GetServiceAsync(team, subscriptionId);

        // Assert  
        result.Succeeded.ShouldBeFalse();
        result.NotFound.ShouldBeTrue();
    }

    //------------------------------------//  
}
