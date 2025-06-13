using ID.Application.Features.FeatureFlags;
using ID.Application.Mediatr.Cqrslmps.Queries;

namespace ID.Application.Features.FeatureFlags.Qry.GetById;
public record GetFeatureFlagByIdQry(Guid? Id) : AIdQuery<FeatureFlagDto>;