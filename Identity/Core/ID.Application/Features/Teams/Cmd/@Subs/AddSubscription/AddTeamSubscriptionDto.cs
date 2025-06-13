namespace ID.Application.Features.Teams.Cmd.Subs.AddSubscription;
public record AddTeamSubscriptionDto(Guid TeamId, Guid SubscriptionPlanId, double? Discount = null);



