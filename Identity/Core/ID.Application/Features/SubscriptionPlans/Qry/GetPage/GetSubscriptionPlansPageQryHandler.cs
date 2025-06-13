using MyResults;
using Pagination;
using ID.Application.Features.SubscriptionPlans;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Abstractions.Services.SubPlans;

namespace ID.Application.Features.SubscriptionPlans.Qry.GetPage;
internal class GetSubscriptionPlansPageQryHandler(IIdentitySubscriptionPlanService _repo)
    : IIdQueryHandler<GetSubscriptionPlansPageQry, PagedResponse<SubscriptionPlanDto>>
{
    public async Task<GenResult<PagedResponse<SubscriptionPlanDto>>> Handle(GetSubscriptionPlansPageQry request, CancellationToken cancellationToken)
    {

        var pgRequest = request.PagedRequest ?? PagedRequest.Empty();

        var page = await _repo.GetPageAsync(pgRequest.PageNumber, pgRequest.PageSize, pgRequest.SortList, pgRequest.FilterList);

        var response = new PagedResponse<SubscriptionPlanDto>(
            page.Transform((d) => d.ToDto()),
            pgRequest
        );

        return GenResult<PagedResponse<SubscriptionPlanDto>>.Success(response);

    }

}//Cls

