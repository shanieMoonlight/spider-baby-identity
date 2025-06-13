using ID.Domain.Entities.Teams;
using ID.Domain.Utility.Messages;
using ID.Infrastructure.DomainServices.Teams.Subs;
using ID.Tests.Data.Factories;
using Shouldly;

namespace ID.Infrastructure.Tests.DmnSevcs.SubsService;
public class RemoveSubscriptionTests : BaseSubServiceTests
{
    //------------------------------------//

    [Fact]
    public async Task RemoveSubscriptionAsync_Success_ReturnsSuccessfulGenResult()
    {
        var existingSubscriptionId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var teamData = TeamDataGenerator.CreateTeamWithSubs(teamId, existingSubscriptionId);
        var sub = teamData.Subscription;
        var plan = teamData.SubscriptionPlan;
        var team = teamData.Team;
        var subCount = team.Subscriptions.Count;
        var service = new SubscriptionService(_uowMock.Object, team);
        //_teamMock.Setup(t => t.TeamSubscriptions).Returns(new List<TeamSubscription>() { sub });
        _uowMock.Setup(uow => uow.SaveChangesAsync(default)).Returns(Task.FromResult(1));

        var result = await service.RemoveSubscriptionAsync(existingSubscriptionId);

        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeTrue();

    }

    //------------------------------------//

    [Fact]
    public async Task RemoveSubscriptionAsync_Success_ReturnsSuccessfulGenResult_2()
    {
        var existingSubscriptionId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var teamData = TeamDataGenerator.CreateTeamWithSubs(teamId, existingSubscriptionId);
        var sub = teamData.Subscription;
        var plan = teamData.SubscriptionPlan;
        var team = teamData.Team;
        var subCount = team.Subscriptions.Count;
        var service = new SubscriptionService(_uowMock.Object, team);
        //_teamMock.Setup(t => t.TeamSubscriptions).Returns(new List<TeamSubscription>() { sub });
        _uowMock.Setup(uow => uow.SaveChangesAsync(default)).Returns(Task.FromResult(1));

        var result = await service.RemoveSubscriptionAsync(sub);

        result.Value.ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public async Task RemoveSubscriptionAsync_NotFound_ReturnsNotFound()
    {
        var NON_existingSubscriptionId = Guid.NewGuid();
        var existingSubscriptionId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var teamData = TeamDataGenerator.CreateTeamWithSubs(teamId, existingSubscriptionId);
        var team = teamData.Team;

        var service = new SubscriptionService(_uowMock.Object, team);
        var result = await service.RemoveSubscriptionAsync(NON_existingSubscriptionId);

        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeFalse();
        result.NotFound.ShouldBeTrue();
        result.Info.ShouldContain(IDMsgs.Error.NotFound<TeamSubscription>(NON_existingSubscriptionId));
    }

    //------------------------------------//

    [Fact]
    public async Task RemoveSubscriptionAsync_NotFound_ReturnsFalse()
    {
        var NON_existingSubscriptionId = Guid.NewGuid();
        var existingSubscriptionId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var teamData = TeamDataGenerator.CreateTeamWithSubs(teamId, existingSubscriptionId);
        var team = teamData.Team;
        var nonExistingSub = SubscriptionDataFactory.Create();

        var service = new SubscriptionService(_uowMock.Object, team);
        var result = await service.RemoveSubscriptionAsync(nonExistingSub);

        result.Value.ShouldBeFalse();
    }

    //------------------------------------//

}//Cls
