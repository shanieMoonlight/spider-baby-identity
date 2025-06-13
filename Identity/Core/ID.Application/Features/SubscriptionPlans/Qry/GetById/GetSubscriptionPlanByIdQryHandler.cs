using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Abstractions.Services.SubPlans;
using ID.Domain.Entities.SubscriptionPlans;
using ID.Domain.Utility.Messages;
using MyResults;

namespace ID.Application.Features.SubscriptionPlans.Qry.GetById;
internal class GetSubscriptionPlanByIdQryHandler(IIdentitySubscriptionPlanService _repo) : IIdQueryHandler<GetSubscriptionPlanByIdQry, SubscriptionPlanDto>
{

    public async Task<GenResult<SubscriptionPlanDto>> Handle(GetSubscriptionPlanByIdQry request, CancellationToken cancellationToken)
    {
        var id = request.Id;
        var mdl = await _repo.GetByIdWithFeatureFlagsAsync(id);

        if (mdl == null)
            return GenResult<SubscriptionPlanDto>.NotFoundResult(IDMsgs.Error.NotFound<SubscriptionPlan>(id));

        return GenResult<SubscriptionPlanDto>.Success(mdl.ToDto());

    }


}//Cls
