using ID.Domain.Abstractions.Events;

namespace ID.Domain.Entities.SubscriptionPlans.Events;
public sealed record SubscriptionPlanFeatureRemovedDomainEvent(Guid SubscriptionPlanId, Guid FeatureFlagId) : IIdDomainEvent;