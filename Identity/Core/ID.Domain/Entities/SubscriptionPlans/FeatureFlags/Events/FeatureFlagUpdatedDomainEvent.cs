using ID.Domain.Abstractions.Events;

namespace ID.Domain.Entities.SubscriptionPlans.FeatureFlags.Events;
public sealed record FeatureFlagUpdatedDomainEvent(Guid FlagId) : IIdDomainEvent;