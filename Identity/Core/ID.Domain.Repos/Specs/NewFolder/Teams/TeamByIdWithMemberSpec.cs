using Microsoft.EntityFrameworkCore;
using ClArch.SimpleSpecification;
using ID.Domain.Entities.Teams;

namespace ID.Infrastructure.Persistance.EF.Repos.Specs.Teams;
/// <summary>
/// Specification for querying a team by its ID and including a specific member with their address.
/// </summary>
internal class TeamByIdWithMemberSpec : ASimpleSpecification<Team>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TeamByIdWithMemberSpec"/> class.
    /// </summary>
    /// <param name="teamId">The ID of the team to query.</param>
    /// <param name="userId">The ID of the user to include in the query.</param>
    public TeamByIdWithMemberSpec(Guid? teamId, Guid? userId)
        : base(tm => tm.Id == teamId)
    {
        // Set short circuit condition to avoid query execution if teamId or userId is not provided
        SetShortCircuit(() => !teamId.HasValue || !userId.HasValue);

        // Include the specified member and their address in the query
        SetInclude(query =>
            query.Include(e => e.Members.Where(m => m.Id == userId))
                .ThenInclude(m => m.Address)
        );
    }
}
