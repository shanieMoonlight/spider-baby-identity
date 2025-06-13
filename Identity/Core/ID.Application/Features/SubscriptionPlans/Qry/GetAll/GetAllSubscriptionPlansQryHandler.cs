using MyResults;
using ID.Application.Features.SubscriptionPlans;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Abstractions.Services.SubPlans;

namespace ID.Application.Features.SubscriptionPlans.Qry.GetAll;
internal class GetAllSubscriptionPlansQryHandler(IIdentitySubscriptionPlanService _repo)
    : IIdQueryHandler<GetAllSubscriptionPlansQry, IEnumerable<SubscriptionPlanDto>>
{

    public async Task<GenResult<IEnumerable<SubscriptionPlanDto>>> Handle(GetAllSubscriptionPlansQry request, CancellationToken cancellationToken)
    {
        var mdls = await _repo.GetAllAsync();
        var dtos = mdls.Select(mdl => mdl.ToDto());
        return GenResult<IEnumerable<SubscriptionPlanDto>>.Success(dtos);

    }

}//Cls
