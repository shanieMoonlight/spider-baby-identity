using ClArch.SimpleSpecification;
using ID.Domain.Entities.SubscriptionPlans;

namespace ID.Infrastructure.Tests.Persistence.EF.Repos.SubscriptionPlans;

public class GetSubscriptionPlanByNameSpec : ASimpleSpecification<SubscriptionPlan> 
{
    internal GetSubscriptionPlanByNameSpec(string? name) : base(p => p.Name == name)
    {
        SetShortCircuit(() => string.IsNullOrWhiteSpace(name));
    }
}
