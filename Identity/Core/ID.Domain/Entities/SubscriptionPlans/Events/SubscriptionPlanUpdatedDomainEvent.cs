using ID.Domain.Abstractions.Events;
using ID.Domain.Entities.SubscriptionPlans;

namespace ID.Domain.Entities.SubscriptionPlans.Events;
public sealed record SubscriptionPlanUpdatedDomainEvent(Guid PlanId, SubscriptionPlan Plan) : IIdDomainEvent 
{
    
}
