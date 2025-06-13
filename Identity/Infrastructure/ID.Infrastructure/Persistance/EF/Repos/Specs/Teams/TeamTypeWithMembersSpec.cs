using ClArch.SimpleSpecification;
using ID.Domain.Entities.Teams;
using Microsoft.EntityFrameworkCore;

namespace ID.Infrastructure.Persistance.EF.Repos.Specs.Teams;

//#################################################################//


/// <summary>
/// Specification for querying teams of a specific type with members up to a certain position.
/// </summary>
internal class TeamTypeWithMembersSpec : ASimpleSpecification<Team>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TeamTypeWithMembersSpec"/> class.
    /// </summary>
    /// <param name="teamType">The type of the team.</param>
    /// <param name="maxPosition">The maximum position of the members to include.</param>
    public TeamTypeWithMembersSpec(TeamType teamType, int maxPosition) : base(r => r.TeamType == teamType)
    {
        SetInclude(query => query
            .Include(b => b.Members.Where(m => m.TeamPosition <= maxPosition))
        );
    }
}


//#################################################################//


/// <summary>
/// Specification for querying super teams with members up to a certain position.
/// </summary>
internal class SuperTeamWithMembersSpec(int maxPosition)
    : TeamTypeWithMembersSpec(TeamType.Super, maxPosition)
{ }


//#################################################################//


/// <summary>
/// Specification for querying maintenance teams with members up to a certain position.
/// </summary>
internal class MntcTeamWithMembersSpec(int maxPosition)
    : TeamTypeWithMembersSpec(TeamType.Maintenance, maxPosition)
{ }


//#################################################################//
