using ID.Domain.Abstractions.Events;

namespace ID.Domain.Entities.SubscriptionPlans.Events;
public sealed record SubscriptionPlanFeatureAddedDomainEvent(Guid SubscriptionPlanId,  Guid FeatureFlagId) : IIdDomainEvent;

public sealed record SubscriptionPlanFeaturesAddedDomainEvent(Guid SubscriptionPlanId, IEnumerable<Guid> FeatureFlagIds) : IIdDomainEvent;
