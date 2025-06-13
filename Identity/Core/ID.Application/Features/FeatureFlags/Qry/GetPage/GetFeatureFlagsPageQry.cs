using ID.Application.Features.FeatureFlags;
using ID.Application.Mediatr.Cqrslmps.Queries;
using Pagination;

namespace ID.Application.Features.FeatureFlags.Qry.GetPage;
public record GetFeatureFlagsPageQry(PagedRequest? PagedRequest) : AIdQuery<PagedResponse<FeatureFlagDto>>;