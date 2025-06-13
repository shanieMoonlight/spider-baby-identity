using ID.Domain.Entities.SubscriptionPlans;
using ID.Domain.Entities.Teams;
using ID.Domain.Utility.Messages;
using ID.Infrastructure.DomainServices.Teams.Subs;
using Shouldly;
using Xunit.Abstractions;

namespace ID.Infrastructure.Tests.DmnSevcs.SubsService;
public class RecordSubscriptionPaymentTests(ITestOutputHelper outputHelper) : BaseSubServiceTests
{
    //------------------------------------//

    [Fact]
    public async Task RecordSubscriptionPaymentAsync_Success_ReturnsSuccessfulGenResult()
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

        var result = await service.RecordPaymentAsync(existingSubscriptionId);

        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldBe(sub);
        result.Value!.LastPaymenAmount.ShouldBe(plan.Price);
        //result.Value!.LastPaymentDate.Date.ShouldBe(DateTime.Now.Date);
        var extendedDate = DateTime.Now.Extend(plan.RenewalType)?.Date;
        DateTime? newEndDate = result.Value!.EndDate?.Date;

        outputHelper.WriteLine($"newEndDate: {newEndDate}");
        outputHelper.WriteLine($"extendedDate: {extendedDate}");
        outputHelper.WriteLine($"plan: {plan.RenewalType}");
        outputHelper.WriteLine($"sub.plan: {sub.RenewalType}");

        newEndDate.ShouldBe(extendedDate);
    }

    //------------------------------------//

    [Fact]
    public async Task RecordSubscriptionPaymentAsync_Success_ReturnsSuccessfulGenResult_2()
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

        var result = await service.RecordPaymentAsync(sub);

        result.ShouldNotBeNull();
        result.ShouldBe(sub);
        result.LastPaymenAmount.ShouldBe(plan.Price);
        result.LastPaymentDate.Date.ShouldBe(DateTime.Now.Date);
        result.EndDate?.Date.ShouldBe(DateTime.Now.Extend(plan.RenewalType)!.Value.Date);
    }

    //------------------------------------//

    [Fact]
    public async Task RecordSubscriptionPaymentAsync_NotFound_ReturnsNotFound()
    {
        var NON_existingSubscriptionId = Guid.NewGuid();
        var existingSubscriptionId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var teamData = TeamDataGenerator.CreateTeamWithSubs(teamId, existingSubscriptionId);
        var team = teamData.Team;

        var service = new SubscriptionService(_uowMock.Object, team);
        var result = await service.RecordPaymentAsync(NON_existingSubscriptionId);

        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeFalse();
        result.NotFound.ShouldBeTrue();
        result.Info.ShouldContain(IDMsgs.Error.NotFound<TeamSubscription>(NON_existingSubscriptionId));
    }

    //------------------------------------//

}//Cls
