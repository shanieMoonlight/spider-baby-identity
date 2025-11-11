using ClArch.SimpleSpecification;
using ID.Domain.Entities.SubscriptionPlans.FeatureFlags;

namespace ID.Infrastructure.Persistance.EF.Repos.Specs.FeatureFlags;

/// <summary>
/// Specification for querying FeatureFlags by name.
/// </summary>
internal class FlagByNameSpec : ASimpleSpecification<FeatureFlag>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FlagByNameSpec"/> class.
    /// </summary>
    /// <param name="name">The name to query by.</param>
    public FlagByNameSpec(string? name)
        : base(e => e.Name.ToLower().Contains(name!.ToLower()))
    {
        SetShortCircuit(() => string.IsNullOrWhiteSpace(name));
    }
}
