using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Abstractions.Services.SubPlans;
using MyResults;

namespace ID.Application.Features.SubscriptionPlans.Qry.GetFiltered;
internal class GetAllSubscriptionPlansFilteredQryHandler(IIdentitySubscriptionPlanService _repo)
    : IIdQueryHandler<GetAllSubscriptionPlansFilteredQry, IEnumerable<SubscriptionPlanDto>>
{

    public async Task<GenResult<IEnumerable<SubscriptionPlanDto>>> Handle(GetAllSubscriptionPlansFilteredQry request, CancellationToken cancellationToken)
    {
        var mdls = await _repo.GetAllByNameAsync(request.Filter);
        var dtos = mdls.Select(mdl => mdl.ToDto());
        return GenResult<IEnumerable<SubscriptionPlanDto>>.Success(dtos);

    }

}//Cls

