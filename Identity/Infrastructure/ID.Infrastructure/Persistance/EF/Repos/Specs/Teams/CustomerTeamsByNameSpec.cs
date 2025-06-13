using ClArch.SimpleSpecification;
using ID.Domain.Entities.Teams;
using Microsoft.EntityFrameworkCore;

namespace ID.Infrastructure.Persistance.EF.Repos.Specs.Teams;


/// <summary>
/// Specification for querying customer teams by name.
/// </summary>
internal class CustomerTeamsByNameSpec : ASimpleSpecification<Team>
{

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomerTeamsByNameSpec"/> class.
    /// </summary>
    /// <param name="name">The name of the team to query.</param>
    public CustomerTeamsByNameSpec(string? name)
        : base(tm =>
            tm.Name.ToLower().Contains(name!.ToLower())
            &&
            tm.TeamType == TeamType.Customer)
    {

        // Set short circuit condition to avoid query execution if name is not provided
        SetShortCircuit(() => string.IsNullOrWhiteSpace(name));

        // Include the specified member and their address in the query
        SetInclude(query => query
            .Include(e => e.Members)
                .ThenInclude(m => m.Address)
        );

        // Order the query results by name and description
        SetOrderBy(query =>
            query
                .OrderByDescending(e => e.Name)
                .ThenBy(e => e.Description)
        );

    }


}//Cls
