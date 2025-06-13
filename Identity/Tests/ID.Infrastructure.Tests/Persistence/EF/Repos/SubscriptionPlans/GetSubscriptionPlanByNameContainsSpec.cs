using ClArch.SimpleSpecification;
using ID.Domain.Entities.SubscriptionPlans;

namespace ID.Infrastructure.Tests.Persistence.EF.Repos.SubscriptionPlans;

public class GetSubscriptionPlanByNameContainsSpec : ASimpleSpecification<SubscriptionPlan> 
{
    internal GetSubscriptionPlanByNameContainsSpec(string name) : base(p => p.Name.Contains(name))
    {
        SetShortCircuit(() => string.IsNullOrWhiteSpace(name));
    }
}
