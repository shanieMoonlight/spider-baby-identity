using ID.Domain.Abstractions.Events;

namespace ID.Domain.Entities.SubscriptionPlans.FeatureFlags.Events;
public sealed record FeatureFlagCreatedDomainEvent(Guid FlagId, FeatureFlag Flag) : IIdDomainEvent 
{
    
}
