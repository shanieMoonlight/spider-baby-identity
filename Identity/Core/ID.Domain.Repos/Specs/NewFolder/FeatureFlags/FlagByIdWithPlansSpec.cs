using ID.Domain.Entities.SubscriptionPlans.FeatureFlags;
using ID.Domain.Repos.Specs;
using Microsoft.EntityFrameworkCore;

namespace ID.Infrastructure.Persistance.EF.Repos.Specs.FeatureFlags;
internal class FlagByIdWithPlansSpec : GetByIdSpec<FeatureFlag>
{
    public FlagByIdWithPlansSpec(Guid? id) : base(id)
    {
        SetInclude(query =>
            query.Include(m => m.SubscriptionPlans)
        );
    }

}//Cls
