using ID.Domain.Abstractions.Events;
using ID.Domain.Entities.SubscriptionPlans.FeatureFlags;

namespace ID.Domain.Entities.SubscriptionPlans.Events;
public sealed record SubscriptionPlanFeatureRemovedDomainEvent(
    Guid SubscriptionPlanId, 
    SubscriptionPlan SubscriptionPlan, 
    FeatureFlag FeatureFlag) : IIdDomainEvent 
{
    
}
