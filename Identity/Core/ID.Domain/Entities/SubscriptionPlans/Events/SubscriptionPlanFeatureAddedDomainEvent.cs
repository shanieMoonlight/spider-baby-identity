using ID.Domain.Abstractions.Events;
using ID.Domain.Entities.SubscriptionPlans.FeatureFlags;

namespace ID.Domain.Entities.SubscriptionPlans.Events;
public sealed record SubscriptionPlanFeatureAddedDomainEvent(Guid SubscriptionPlanId, SubscriptionPlan SubscriptionPlan, FeatureFlag FeatureFlag) : IIdDomainEvent 
{
    
}

public sealed record SubscriptionPlanFeaturesAddedDomainEvent(Guid SubscriptionPlanId, SubscriptionPlan SubscriptionPlan, IEnumerable<FeatureFlag> FeatureFlags) : IIdDomainEvent
{
    
}
