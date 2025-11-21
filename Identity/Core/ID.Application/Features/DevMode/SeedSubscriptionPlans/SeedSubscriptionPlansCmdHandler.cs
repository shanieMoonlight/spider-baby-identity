using ID.Application.Features.SubscriptionPlans;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Abstractions.Services.SubPlans;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.SubscriptionPlans;
using ID.Domain.Entities.SubscriptionPlans.ValueObjects;
using MyResults;

namespace ID.Application.Features.DevMode.SeedSubscriptionPlans;
public class SeedSubscriptionPlansCmdHandler(IIdentitySubscriptionPlanService _service)
    : IIdCommandHandler<SeedSubscriptionPlansCmd<AppUser>, List<SubscriptionPlanDto>>
{

    public async Task<GenResult<List<SubscriptionPlanDto>>> Handle(SeedSubscriptionPlansCmd<AppUser> request, CancellationToken cancellationToken)
    {
        var plans = CreateSeedSupsriptionPlans();
        var createdDtos = new List<SubscriptionPlanDto>();

        foreach (var plan in plans)
        {
            var newPlan = await _service.AddAsync(plan, cancellationToken);
            createdDtos.Add(newPlan.ToDto());
        }

        return GenResult<List<SubscriptionPlanDto>>.Success(createdDtos);
    }

    //----------------------//


    private static List<SubscriptionPlan> CreateSeedSupsriptionPlans()
    {
        var plans = new List<SubscriptionPlan>
        {
            SubscriptionPlan.Create(
                Name.Create("Free Plan"),
                Description.Create("Basic access with limited features."),
                Price.Create(0),
                SubscriptionRenewalTypes.Monthly,
                TrialMonths.Create(0),
                DeviceLimit.Create(1)
            ),
            SubscriptionPlan.Create(
                Name.Create("Pro Plan"),
                Description.Create("Advanced features for professionals."),
                Price.Create(9.99),
                SubscriptionRenewalTypes.Monthly,
                TrialMonths.Create(1),
                DeviceLimit.Create(5)
            ),
            SubscriptionPlan.Create(
                Name.Create("Enterprise Plan"),
                Description.Create("All features for enterprise users."),
                Price.Create(199.99),
                SubscriptionRenewalTypes.Annual,
                TrialMonths.Create(1),
                DeviceLimit.Create(0) // 0 interpreted as unlimited in domain model
            )
        };
        return plans;
    }


}//Cls




