namespace ID.Infrastructure.Tests.Persistence.EF.Repos.FeatureFlags;
public class GetFeatureFlagByNameStartsWithSpec : ASimpleSpecification<FeatureFlag> 
{
    internal GetFeatureFlagByNameStartsWithSpec(string? name) : base(f => f.Name.StartsWith(name!))
    {
        SetShortCircuit(() => string.IsNullOrWhiteSpace(name));
    }
}
