using ID.Infrastructure.DomainServices.Teams.Subs;
using Shouldly;

namespace ID.Infrastructure.Tests.DmnSevcs.SubsService;


public class GetSubscriptionTests : BaseSubServiceTests
{

    //------------------------------------//

    [Fact]
    public async Task GetSubscriptionAsync_ExistingSubscription_ReturnsSubscription()
    {
        var teamId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();
        var teamData = TeamDataGenerator.CreateTeamWithSubs(teamId, subscriptionId);
        var team = teamData.Team;
        var service = new SubscriptionService(_uowMock.Object, team);

        var result = await service.GetSubscriptionAsync(subscriptionId);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(subscriptionId);
    }

    //------------------------------------//

    [Fact]
    public async Task GetSubscriptionAsync_NonExistentSubscription_ReturnsNull()
    {
        var teamId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();
        var teamData = TeamDataGenerator.CreateTeamWithSubs(teamId, subscriptionId);
        var team = teamData.Team;
        var service = new SubscriptionService(_uowMock.Object, team);

        var result = await service.GetSubscriptionAsync(Guid.NewGuid());

        result.ShouldBeNull();
    }

    //------------------------------------//

}//Cls
