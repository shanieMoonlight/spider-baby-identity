using MyResults;
using ID.Application.Features.SubscriptionPlans;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Utility.Messages;
using ID.Domain.Entities.SubscriptionPlans;
using ID.Domain.Abstractions.Services.SubPlans;

namespace ID.Application.Features.SubscriptionPlans.Cmd.FeatureFlags.AddFeature;
public class AddFeatureToSubscriptionPlanCmdHandler(IIdentitySubscriptionPlanService _service)
    : IIdCommandHandler<AddFeatureToSubscriptionPlanCmd, SubscriptionPlanDto>
{

    public async Task<GenResult<SubscriptionPlanDto>> Handle(AddFeatureToSubscriptionPlanCmd request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var planId = dto.SubscriptionPlanId;
        var featureIds = dto.FeatureIds;

        var entity = await _service.GetByIdWithFeatureFlagsAsync(planId);
        if (entity is null)
            return GenResult<SubscriptionPlanDto>.NotFoundResult(IDMsgs.Error.NotFound<SubscriptionPlan>(planId));


        var updatePlan = await _service.AddFeaturesToPlanAsync(entity, featureIds, cancellationToken);


        return GenResult<SubscriptionPlanDto>.Success(updatePlan.ToDto());
    }


}//Cls




