using ID.Application.Features.SubscriptionPlans;
using ID.Application.Mediatr.Cqrslmps.Queries;

namespace ID.Application.Features.SubscriptionPlans.Qry.GetById;
public record GetSubscriptionPlanByIdQry(Guid? Id) : AIdQuery<SubscriptionPlanDto>;
