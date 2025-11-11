using ClArch.SimpleSpecification;
using ID.Domain.Entities.AppUsers;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ID.Infrastructure.Persistance.EF.Repos.Specs.Members.WithEverything;

internal abstract class AMemberlWithEverythingSpec<TUser> : ASimpleSpecification<TUser> where TUser : AppUser
{
    public AMemberlWithEverythingSpec(Expression<Func<TUser, bool>> criteria) : base(criteria)
    {
        SetInclude(query => query
             .Include(u => u.Team)
                .ThenInclude(t => t!.Subscriptions)
                    .ThenInclude(t => t.Devices)
                .Include(u => u.Team)
                    .ThenInclude(t => t!.Subscriptions)
                        .ThenInclude(t => t.SubscriptionPlan));
    }

}//Cls
