using ClArch.SimpleSpecification;
using ID.Domain.Entities.SubscriptionPlans;

namespace ID.Infrastructure.Tests.Persistence.EF.Repos.SubscriptionPlans;

/// <summary>
/// Specification for filtering SubscriptionPlans by renewal type.
/// </summary>
public class GetSubscriptionPlansByRenewalTypeSpec : ASimpleSpecification<SubscriptionPlan>
{
    internal GetSubscriptionPlansByRenewalTypeSpec(SubscriptionRenewalTypes renewalType) 
        : base(p => p.RenewalType == renewalType)
    {
    }
}
