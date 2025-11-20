using ID.Domain.Entities.SubscriptionPlans;
using ID.Domain.Repos.Specs;
using Microsoft.EntityFrameworkCore;

namespace ID.Domain.Repos.Specs.SubPlans;
internal class SubPlanByIdWithFlagsSpec : GetByIdSpec<SubscriptionPlan>
{
    public SubPlanByIdWithFlagsSpec(Guid? id) : base(id)
    {
        SetInclude(query =>
            query.Include(m => m.FeatureFlags)
        );
    }

}//Cls
