using ClArch.SimpleSpecification;
using ID.Domain.Entities.SubscriptionPlans;

namespace ID.Infrastructure.Tests.Persistence.EF.Repos.SubscriptionPlans;

/// <summary>
/// Specification for filtering SubscriptionPlans by device limit.
/// </summary>
public class GetSubscriptionPlansByDeviceLimitSpec : ASimpleSpecification<SubscriptionPlan>
{
    internal GetSubscriptionPlansByDeviceLimitSpec(int deviceLimit) 
        : base(p => p.DeviceLimit == deviceLimit)
    {
    }
}
