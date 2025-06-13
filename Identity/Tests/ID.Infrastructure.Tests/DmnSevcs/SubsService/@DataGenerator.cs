using ID.Domain.Entities.SubscriptionPlans;
using ID.Domain.Entities.Teams;
using ID.Tests.Data.Factories;

namespace ID.Infrastructure.Tests.DmnSevcs.SubsService;

internal record TeamWithSubAndPlan(Team Team, TeamSubscription Subscription, SubscriptionPlan SubscriptionPlan);
internal class TeamDataGenerator

{
    internal static TeamWithSubAndPlan CreateTeamWithSubs(Guid teamId, Guid subscriptionId, Guid? subscriptionPlanId = null)
    {
        var dvcCount = 2;
        var dvcs = DeviceDataFactory.CreateMany(dvcCount).ToHashSet();
        var plan = SubscriptionPlanDataFactory.Create(subscriptionPlanId);
        var subscription = SubscriptionDataFactory.Create(subscriptionId, null, null, null, subscriptionPlanId, null, 0, dvcs, "TestPlan", "Blah Blah...", plan.RenewalType, plan);
        var otherSubscription = SubscriptionDataFactory.Create(null, null, null, null, null, null, 0);
        var team = TeamDataFactory.Create(teamId, null, null, [subscription, otherSubscription]);
        return new TeamWithSubAndPlan(team, subscription, plan);
    }
}
