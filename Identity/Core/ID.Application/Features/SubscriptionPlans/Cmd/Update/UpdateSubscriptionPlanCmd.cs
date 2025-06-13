using ID.Application.Features.SubscriptionPlans;
using ID.Application.Mediatr.Cqrslmps.Commands;

namespace ID.Application.Features.SubscriptionPlans.Cmd.Update;
public record UpdateSubscriptionPlanCmd(SubscriptionPlanDto Dto) : AIdCommand<SubscriptionPlanDto>;



