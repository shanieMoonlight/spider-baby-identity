using ID.Application.Features.FeatureFlags;
using ID.Application.Mediatr.Cqrslmps.Queries;

namespace ID.Application.Features.FeatureFlags.Qry.GetAll;
public record GetAllFeatureFlagsQry : AIdQuery<IEnumerable<FeatureFlagDto>>;
