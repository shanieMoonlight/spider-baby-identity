using ID.Domain.Abstractions.Events;
using ID.Domain.Entities.Teams;

namespace ID.Domain.Entities.SubscriptionPlans.Subscriptions.Events;
public sealed record SubscriptionDeviceRemovedDomainEvent(Guid PlanId, TeamSubscription Subscription, TeamDevice Device) : IIdDomainEvent
{
    
}
