using ID.Application.Mediatr.Cqrslmps.Queries;

namespace ID.Application.Features.FeatureFlags.Qry.GetAllByName;
public record class GetAllFeatureFlagsByNameQry(string? Name) : AIdQuery<IEnumerable<FeatureFlagDto>>;

