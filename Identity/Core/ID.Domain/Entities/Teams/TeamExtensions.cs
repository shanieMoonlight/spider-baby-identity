namespace ID.Domain.Entities.Teams;
public static class TeamExtensions
{

    public static bool IsSuper(this Team team) =>
        team.TeamType == TeamType.super;

    //- - - - - - - - - - - - //

    public static bool IsMntc(this Team team) =>
        team.TeamType == TeamType.maintenance;

    //- - - - - - - - - - - - //

    public static bool IsCustomer(this Team team) =>
        team.TeamType == TeamType.customer;

    //------------------------//

    public static TeamSubscription? GetSubscription(this Team team, Guid subscriptionPlanId) =>
           team.Subscriptions.FirstOrDefault(s => s.SubscriptionPlanId == subscriptionPlanId);


}//Cls
