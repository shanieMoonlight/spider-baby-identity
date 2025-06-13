using ID.Domain.Entities.SubscriptionPlans;
using ID.Infrastructure.Persistance.Abstractions.Repos.Specs;
using Microsoft.EntityFrameworkCore;

namespace ID.Infrastructure.Persistance.EF.Repos.Specs.SubPlans;
internal class SubPlanByIdWithFlagsSpec : GetByIdSpec<SubscriptionPlan>
{
    public SubPlanByIdWithFlagsSpec(Guid? id) : base(id)
    {
        SetInclude(query =>
            query.Include(m => m.FeatureFlags)
        );
    }

}//Cls
