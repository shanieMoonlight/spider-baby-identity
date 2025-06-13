using MyResults;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Utility.Messages;
using ID.Domain.Entities.SubscriptionPlans;
using ID.Domain.Abstractions.Services.SubPlans;

namespace ID.Application.Features.SubscriptionPlans.Cmd.Delete;
public class DeleteSubscriptionPlanCmdHandler(IIdentitySubscriptionPlanService _repo) : IIdCommandHandler<DeleteSubscriptionPlanCmd>
{

    public async Task<BasicResult> Handle(DeleteSubscriptionPlanCmd request, CancellationToken cancellationToken)
    {
        var id = request.Id;

        await _repo.DeleteAsync(id, cancellationToken);

        return BasicResult.Success(IDMsgs.Info.Deleted<SubscriptionPlan>(id));

    }


}//Cls

