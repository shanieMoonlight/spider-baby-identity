using ID.Domain.Abstractions.Events;

namespace ID.Domain.Entities.SubscriptionPlans.Events;
public sealed record SubscriptionPlanUpdatedDomainEvent(Guid PlanId) : IIdDomainEvent;
