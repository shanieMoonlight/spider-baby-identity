using ID.Domain.Abstractions.Events;
using ID.Domain.Entities.SubscriptionPlans.FeatureFlags;

namespace ID.Domain.Entities.SubscriptionPlans.FeatureFlags.Events;
public sealed record FeatureFlagUpdatedDomainEvent(Guid FlagId, FeatureFlag Flag) : IIdDomainEvent
{
    
}
