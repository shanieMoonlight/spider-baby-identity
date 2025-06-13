using ClArch.SimpleSpecification;
using ID.Domain.Entities.SubscriptionPlans;

namespace ID.Infrastructure.Tests.Persistence.EF.Repos.SubscriptionPlans;

/// <summary>
/// Specification for filtering SubscriptionPlans by price range.
/// </summary>
public class GetSubscriptionPlansByPriceRangeSpec : ASimpleSpecification<SubscriptionPlan>
{
    internal GetSubscriptionPlansByPriceRangeSpec(double minPrice, double maxPrice) 
        : base(p => p.Price >= minPrice && p.Price <= maxPrice)
    {
    }
}
