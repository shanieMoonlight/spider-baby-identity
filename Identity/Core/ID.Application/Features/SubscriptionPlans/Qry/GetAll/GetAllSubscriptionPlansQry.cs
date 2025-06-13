using ID.Application.Features.SubscriptionPlans;
using ID.Application.Mediatr.Cqrslmps.Queries;

namespace ID.Application.Features.SubscriptionPlans.Qry.GetAll;
public record GetAllSubscriptionPlansQry : AIdQuery<IEnumerable<SubscriptionPlanDto>>;
