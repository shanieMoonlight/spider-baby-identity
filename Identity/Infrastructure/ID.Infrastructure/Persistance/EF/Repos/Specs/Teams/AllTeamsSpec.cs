using ClArch.SimpleSpecification;
using ID.Domain.Entities.Teams;
using Microsoft.EntityFrameworkCore;

namespace ID.Infrastructure.Persistance.EF.Repos.Specs.Teams;
public class AllTeamsSpec : ASimpleSpecification<Team>
{
    public AllTeamsSpec() : base()
    {
        SetInclude(query =>
            query.Include(e => e.Members)
                .ThenInclude(m => m.Address)
        );

        SetOrderBy(query =>
            query
                .OrderByDescending(e => e.Name)
                .ThenBy(e => e.Description)
        );
    }

}//Cls
