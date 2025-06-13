using ClArch.SimpleSpecification;
using ID.Domain.Entities.SubscriptionPlans;
using Microsoft.EntityFrameworkCore;

namespace ID.Infrastructure.Persistance.EF.Repos.Specs.SubPlans;

/// <summary>
/// Specification for querying FeatureFlags by name.
/// </summary>
internal class SubPlanByNameSpec : ASimpleSpecification<SubscriptionPlan>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SubPlanByNameSpec"/> class.
    /// </summary>
    /// <param name="name">The name to query by.</param>
    public SubPlanByNameSpec(string? name)
        : base(e => e.Name.ToLower().Contains(name!.ToLower()))
    {
        SetShortCircuit(() => string.IsNullOrWhiteSpace(name));

        SetInclude(query =>
            query.Include(m => m.FeatureFlags)
        );
    }
}
