using MyResults;
using Pagination;
using ID.Application.Features.FeatureFlags;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Abstractions.Services.SubPlans;

namespace ID.Application.Features.FeatureFlags.Qry.GetPage;

internal class GetFeatureFlagsPageQryHandler(IIdentityFeatureFlagService repo) : IIdQueryHandler<GetFeatureFlagsPageQry, PagedResponse<FeatureFlagDto>>
{

    public async Task<GenResult<PagedResponse<FeatureFlagDto>>> Handle(GetFeatureFlagsPageQry request, CancellationToken cancellationToken)
    {
        var pgRequest = request.PagedRequest ?? PagedRequest.Empty();

        var page = (await repo.GetPageAsync(pgRequest.PageNumber, pgRequest.PageSize, pgRequest.SortList, pgRequest.FilterList))
                   .Transform((d) => d.ToDto());

        var response = new PagedResponse<FeatureFlagDto>(page, pgRequest);

        return  GenResult<PagedResponse<FeatureFlagDto>>.Success(response);

    }


}//Cls
