namespace ID.Infrastructure.Tests.Persistence.EF.Repos.FeatureFlags;
public class GetFeatureFlagByNameSpec : ASimpleSpecification<FeatureFlag> 
{
    internal GetFeatureFlagByNameSpec(string? name) : base(f => f.Name == name)
    {
        SetShortCircuit(() => String.IsNullOrWhiteSpace(name));
    }
}
