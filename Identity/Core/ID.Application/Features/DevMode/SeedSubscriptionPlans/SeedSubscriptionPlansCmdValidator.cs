using ID.Application.Mediatr.Validation;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Features.DevMode.SeedSubscriptionPlans;
public class SeedSubscriptionPlansCmdValidator<TUser> : AMntcMinimumValidator<SeedSubscriptionPlansCmd<TUser>>
    where TUser : AppUser
{
    public SeedSubscriptionPlansCmdValidator()
    { }
}//Cls

