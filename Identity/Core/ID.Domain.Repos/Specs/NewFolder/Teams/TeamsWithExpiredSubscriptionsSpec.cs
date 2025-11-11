
using ClArch.SimpleSpecification;
using ID.Domain.Entities.Teams;
using Microsoft.EntityFrameworkCore;

namespace ID.Infrastructure.Persistance.EF.Repos.Specs.Teams;
public class TeamsWithExpiredSubscriptionsSpec : ASimpleSpecification<Team>
{
    public TeamsWithExpiredSubscriptionsSpec() 
        : base(r => 
            r.Subscriptions != null 
            && 
            r.Subscriptions.Any(s => s.EndDate < DateTime.UtcNow)
        )
    {
        SetInclude(query =>
            query
            .Include(e => e.Leader)
            .Include(e => e.Subscriptions.Where(s => s.EndDate < DateTime.UtcNow))
        );

        SetOrderBy(query =>
            query
                .OrderByDescending(e => e.Name)
        );
    }

}//Cls
