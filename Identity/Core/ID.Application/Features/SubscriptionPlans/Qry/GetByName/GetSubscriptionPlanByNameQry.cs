using ID.Application.Features.SubscriptionPlans;
using ID.Application.Mediatr.Cqrslmps.Queries;

namespace ID.Application.Features.SubscriptionPlans.Qry.GetByName;

public record GetSubscriptionPlanByNameQry(string? Name) : AIdQuery<SubscriptionPlanDto>;



