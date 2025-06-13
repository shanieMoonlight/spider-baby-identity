using ID.Domain.Entities.SubscriptionPlans;
using ID.Domain.Entities.SubscriptionPlans.ValueObjects;
using ID.Domain.Entities.Teams;
using ID.Domain.Utility.Messages;
using ID.Infrastructure.DomainServices.Teams.Subs;
using Moq;

namespace ID.Infrastructure.Tests.DmnSevcs.SubsService;


public class AddSubscriptionTests : BaseSubServiceTests
{

    //------------------------------------//

    [Fact]
    public async Task AddSubscriptionAsync_Failure_PlanNotFound_ReturnsNotFound()
    {
        var teamId = Guid.NewGuid();
        var subId = Guid.NewGuid();
        var subPlanId = Guid.NewGuid();
        var teamWithSubs = TeamDataGenerator.CreateTeamWithSubs(teamId, subId, subPlanId);
        var team = teamWithSubs.Team;
        var sub = teamWithSubs.Subscription;
        var plan = teamWithSubs.SubscriptionPlan;
        var originalSubCount = team.Subscriptions.Count;

        var service = new SubscriptionService(_uowMock.Object, team);
        _subPlanRepoMock.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<SubPlanByIdWithFlagsSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((SubscriptionPlan?)null);
        //_subPlanRepoMock.Setup(m => m.GetByIdWithDetailsAsync(plan.Id))
        //    .ReturnsAsync((SubscriptionPlan?)null);


        // Act
        var result = await service.AddSubscriptionAsync(subPlanId, Discount.Create(0));

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldBe(IDMsgs.Error.NotFound<SubscriptionPlan>(subPlanId));

    }

    //------------------------------------//

    [Fact]
    public async Task AddSubscriptionAsync_Success_ReturnsSuccessfulGenResult()
    {
        var teamId = Guid.NewGuid();
        var subId = Guid.NewGuid();
        var subPlanId = Guid.NewGuid();
        var teamWithSubs = TeamDataGenerator.CreateTeamWithSubs(teamId, subId, subPlanId);
        var team = teamWithSubs.Team;
        var sub = teamWithSubs.Subscription;
        var plan = teamWithSubs.SubscriptionPlan;
        var originalSubCount = team.Subscriptions.Count;

        var service = new SubscriptionService(_uowMock.Object, team);

        _subPlanRepoMock.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<SubPlanByIdWithFlagsSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(plan);
        //_subPlanRepoMock.Setup(m => m.GetByIdWithDetailsAsync(plan.Id))
        //    .ReturnsAsync(plan);

        _teamRepoMock.Setup(m => m.UpdateAsync(It.IsAny<Team>()))
            .Verifiable();

        //team.Setup(t => t.AddSubscription(plan, It.Is<Discount>(d => d.Value == cmd.Dto.Discount))).Verifiable(); //Verify discount value


        // Act
        var result = await service.AddSubscriptionAsync(plan.Id, Discount.Create(0));

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeTrue();
        result.Value?.SubscriptionPlanId.ShouldBe(subPlanId); //It whould have been built using the PLan
        result.Value?.Team?.Subscriptions?.Count.ShouldBe(originalSubCount + 1);
        _teamRepoMock.Verify();
        _uowMock.Verify();
    }

    //------------------------------------//

}//Cls

