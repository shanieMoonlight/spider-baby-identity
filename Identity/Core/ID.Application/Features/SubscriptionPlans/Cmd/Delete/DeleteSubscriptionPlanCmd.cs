using ID.Application.Mediatr.Cqrslmps.Commands;

namespace ID.Application.Features.SubscriptionPlans.Cmd.Delete;
public record DeleteSubscriptionPlanCmd(Guid Id) : AIdCommand;



