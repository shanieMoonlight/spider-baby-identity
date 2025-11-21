using ID.Application.Features.SubscriptionPlans;
using ID.Application.Mediatr.Cqrslmps.Commands;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Features.DevMode.DeleteSubscriptionPlans;
public record DeleteSubscriptionPlansCmd<TUser>() 
    : AIdDevModeCommand<TUser, List<SubscriptionPlanDto>> 
    where TUser : AppUser;



