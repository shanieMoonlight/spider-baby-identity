namespace ID.Infrastructure.Tests.Persistence.EF.Repos.FeatureFlags;
public class GetFeatureFlagByNameContainsSpec : ASimpleSpecification<FeatureFlag> 
{
    internal GetFeatureFlagByNameContainsSpec(string name) : base(f => f.Name.Contains(name))
    {
        SetShortCircuit(() => string.IsNullOrWhiteSpace(name));
    }
}
