using CollectionHelpers;
using MyResults;
using ID.Application.Features.SubscriptionPlans;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Abstractions.Services.SubPlans;

namespace ID.Application.Features.SubscriptionPlans.Cmd.Create;
public class CreateSubscriptionPlanCmdHandler(IIdentitySubscriptionPlanService _service)
    : IIdCommandHandler<CreateSubscriptionPlanCmd, SubscriptionPlanDto>
{

    public async Task<GenResult<SubscriptionPlanDto>> Handle(CreateSubscriptionPlanCmd request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var mdl = dto.ToModel();

        var ids = dto.FeatureFlags.AnyValues()
              ? dto.FeatureFlags.Select(f => f.Id)
              : dto.FeatureFlagIds;


        var newPlan = await _service.AddAsync(mdl, ids, cancellationToken);

        return GenResult<SubscriptionPlanDto>.Success(newPlan.ToDto());
    }


}//Cls




