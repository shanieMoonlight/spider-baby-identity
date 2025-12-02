using ClArch.SimpleSpecification;
using ID.Domain.Entities.AppUsers;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ID.Domain.Repos.Specs.Members.WithEverything;

internal abstract class AMemberWithEverythingSpec<TUser> : ASimpleSpecification<TUser> where TUser : AppUser
{
    public AMemberWithEverythingSpec(Expression<Func<TUser, bool>> criteria) : base(criteria)
    {
        SetInclude(query => query
            .Include(u => u.TrustedDevices) 
             .Include(u => u.Team)
                .ThenInclude(t => t!.Subscriptions)
                    .ThenInclude(t => t.Devices)
                .Include(u => u.Team)
                    .ThenInclude(t => t!.Subscriptions)
                        .ThenInclude(t => t.SubscriptionPlan));

    }

}//Cls
