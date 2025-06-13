using MyResults;
using ID.Application.Features.SubscriptionPlans;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Utility.Messages;
using ID.Domain.Entities.SubscriptionPlans;
using ID.Domain.Abstractions.Services.SubPlans;

namespace ID.Application.Features.SubscriptionPlans.Cmd.Update;
public class UpdateSubscriptionPlanCmdHandler(IIdentitySubscriptionPlanService _repo)
    : IIdCommandHandler<UpdateSubscriptionPlanCmd, SubscriptionPlanDto>
{
    public async Task<GenResult<SubscriptionPlanDto>> Handle(UpdateSubscriptionPlanCmd request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var mdl = await _repo.GetByIdWithFeatureFlagsAsync(dto.Id);

        if (mdl == null)
            return GenResult<SubscriptionPlanDto>.NotFoundResult(IDMsgs.Error.NotFound<SubscriptionPlan>(dto.Id));

        mdl.Update(dto);

        var entity = await _repo.UpdateAsync(mdl, cancellationToken);

        return GenResult<SubscriptionPlanDto>.Success(entity!.ToDto());

    }

}//Cls


