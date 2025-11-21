using ID.Application.Mediatr.Validation;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Features.DevMode.DeleteSubscriptionPlans;
public class DeleteSubscriptionPlansCmdValidator<TUser> : AMntcMinimumValidator<DeleteSubscriptionPlansCmd<TUser>>
    where TUser : AppUser
{
    public DeleteSubscriptionPlansCmdValidator()
    { }
}//Cls

