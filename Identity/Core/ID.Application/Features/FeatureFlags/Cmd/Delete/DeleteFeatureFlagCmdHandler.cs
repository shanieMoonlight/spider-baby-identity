using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Abstractions.Services.SubPlans;
using ID.Domain.Entities.SubscriptionPlans.FeatureFlags;
using ID.Domain.Utility.Messages;
using MyResults;

namespace ID.Application.Features.FeatureFlags.Cmd.Delete;


public class DeleteFeatureFlagCmdHandler(IIdentityFeatureFlagService _repo) : IIdCommandHandler<DeleteFeatureFlagCmd>
{

    public async Task<BasicResult> Handle(DeleteFeatureFlagCmd request, CancellationToken cancellationToken)
    {
        var id = request.Id;

        await _repo.DeleteAsync(id);

        return BasicResult.Success(IDMsgs.Info.Deleted<FeatureFlag>(id));
    }

}//Cls
