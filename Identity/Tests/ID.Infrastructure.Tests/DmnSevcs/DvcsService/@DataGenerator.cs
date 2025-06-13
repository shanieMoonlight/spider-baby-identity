using ID.Domain.Entities.SubscriptionPlans;
using ID.Domain.Entities.Teams;
using ID.Tests.Data.Factories;

namespace ID.Infrastructure.Tests.DmnSevcs.DvcsService;

internal record TeamWithSubDvcAndPlan(Team Team, TeamSubscription Subscription, TeamDevice Device, SubscriptionPlan SubscriptionPlan);
internal class TeamDataGenerator

{
    internal static TeamWithSubDvcAndPlan CreateTeamWithSubsAndDvcs(
        Guid teamId,
        Guid subscriptionId,
        Guid subscriptionPlanId,
        int dvcLimit = 0,
        Guid? dvcId = null)
    {
        var plan = SubscriptionPlanDataFactory.Create(subscriptionPlanId);
        var dvc = DeviceDataFactory.Create(dvcId, subscriptionId);
        var dvcs = DeviceDataFactory.CreateMany(2);
        dvcs.Add(dvc);

        var subscription = SubscriptionDataFactory.Create(
            id: subscriptionId,
          subscriptionPlanId: subscriptionPlanId,
            deviceLimit: dvcLimit,
            devices: [.. dvcs],
           name: "TestPlan",
            description: "Blah Blah...",
           renewalType: plan.RenewalType,
           plan: plan);

        var otherSubscription = SubscriptionDataFactory.Create(null, null, null, null, null, null, 0, null, null);
        var team = TeamDataFactory.Create(teamId, null, null, [subscription, otherSubscription]);
        return new TeamWithSubDvcAndPlan(team, subscription, dvc, plan);
    }
}
