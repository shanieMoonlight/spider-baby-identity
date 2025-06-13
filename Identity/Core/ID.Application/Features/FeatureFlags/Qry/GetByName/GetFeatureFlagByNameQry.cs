using ID.Application.Features.FeatureFlags;
using ID.Application.Mediatr.Cqrslmps.Queries;

namespace ID.Application.Features.FeatureFlags.Qry.GetByName;
public record class GetFeatureFlagByNameQry(string? Name) : AIdQuery<FeatureFlagDto>;

