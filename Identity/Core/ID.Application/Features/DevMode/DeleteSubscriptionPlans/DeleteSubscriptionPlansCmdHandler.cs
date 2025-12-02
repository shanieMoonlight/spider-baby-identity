using ID.Application.Features.SubscriptionPlans;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Abstractions.Services.SubPlans;
using ID.Domain.Entities.AppUsers;
using MyResults;

namespace ID.Application.Features.DevMode.DeleteSubscriptionPlans;
public class DeleteSubscriptionPlansCmdHandler(IIdentitySubscriptionPlanService _service)
    : IIdCommandHandler<DeleteSubscriptionPlansCmd<AppUser>, List<SubscriptionPlanDto>>
{

    public async Task<GenResult<List<SubscriptionPlanDto>>> Handle(DeleteSubscriptionPlansCmd<AppUser> request, CancellationToken cancellationToken)
    {
        var plans = await _service.ListAllAsync();
        var deletedDtos = new List<SubscriptionPlanDto>();

        foreach (var plan in plans)
        {
            await _service.DeleteAsync(plan, cancellationToken);
            deletedDtos.Add(plan.ToDto());
        }

        return GenResult<List<SubscriptionPlanDto>>.Success(deletedDtos);
    }


}//Cls




