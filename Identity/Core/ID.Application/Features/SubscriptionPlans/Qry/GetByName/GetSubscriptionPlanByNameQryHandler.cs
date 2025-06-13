using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Abstractions.Services.SubPlans;
using ID.Domain.Entities.SubscriptionPlans;
using ID.Domain.Utility.Messages;
using MyResults;

namespace ID.Application.Features.SubscriptionPlans.Qry.GetByName;
internal class GetSubscriptionPlanByNameQryHandler(IIdentitySubscriptionPlanService _repo)
    : IIdQueryHandler<GetSubscriptionPlanByNameQry, SubscriptionPlanDto>
{
    public async Task<GenResult<SubscriptionPlanDto>> Handle(GetSubscriptionPlanByNameQry request, CancellationToken cancellationToken)
    {
        var name = request.Name;
        var mdl = await _repo.FirstByNameAsync(name);
        if (mdl == null)
            return GenResult<SubscriptionPlanDto>.NotFoundResult(IDMsgs.Error.NotFound<SubscriptionPlan>(name));

        return GenResult<SubscriptionPlanDto>.Success(mdl.ToDto());

    }

}//Cls

