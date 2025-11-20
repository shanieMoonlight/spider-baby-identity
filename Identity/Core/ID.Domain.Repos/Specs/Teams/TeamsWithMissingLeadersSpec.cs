using ClArch.SimpleSpecification;
using ID.Domain.Entities.Teams;
using Microsoft.EntityFrameworkCore;

namespace ID.Domain.Repos.Specs.Teams;
public class TeamsWithMissingLeadersSpec : ASimpleSpecification<Team>
{
    public TeamsWithMissingLeadersSpec() : base(r => r.LeaderId == null)
    {
        SetInclude(query =>
            query.Include(e => e.Members)
        );

        SetOrderBy(query =>
            query
                .OrderByDescending(e => e.Name)
        );
    }

}//Cls
