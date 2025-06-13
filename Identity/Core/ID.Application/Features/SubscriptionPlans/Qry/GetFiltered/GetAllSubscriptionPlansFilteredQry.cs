using ID.Application.Features.SubscriptionPlans;
using ID.Application.Mediatr.Cqrslmps.Queries;

namespace ID.Application.Features.SubscriptionPlans.Qry.GetFiltered;

public record GetAllSubscriptionPlansFilteredQry(string? Filter) : AIdQuery<IEnumerable<SubscriptionPlanDto>>;


