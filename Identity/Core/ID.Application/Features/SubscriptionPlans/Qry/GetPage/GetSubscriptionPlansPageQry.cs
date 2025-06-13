using ID.Application.Features.SubscriptionPlans;
using ID.Application.Mediatr.Cqrslmps.Queries;
using Pagination;

namespace ID.Application.Features.SubscriptionPlans.Qry.GetPage;

public record GetSubscriptionPlansPageQry(PagedRequest? PagedRequest) : AIdQuery<PagedResponse<SubscriptionPlanDto>>;


