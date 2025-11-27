namespace ID.Infrastructure.Tests.DmnSevcs.SubsService;

internal record TeamWithSubAndPlan(Team Team, TeamSubscription Subscription, SubscriptionPlan SubscriptionPlan);
internal class TeamDataGenerator

{
    internal static TeamWithSubAndPlan CreateTeamWithSubs(Guid teamId, Guid subscriptionId, Guid? subscriptionPlanId = null)
    {
        var dvcCount = 2;
        var dvcs = DeviceDataFactory.CreateMany(dvcCount).ToHashSet();
        var plan = SubscriptionPlanDataFactory.Create(subscriptionPlanId);
        var subscription = SubscriptionDataFactory.Create(
            id: subscriptionId,
            subscriptionPlanId:subscriptionPlanId, 
            deviceLimit:0, 
            devices:dvcs, 
            name:"TestPlan", 
            description:"Blah Blah...", 
            renewalType:plan.RenewalType, 
            plan:plan);
        var otherSubscription = SubscriptionDataFactory.Create( deviceLimit: 0);
        var team = TeamDataFactory.Create(teamId, null, null, [subscription, otherSubscription]);
        return new TeamWithSubAndPlan(team, subscription, plan);
    }
}
