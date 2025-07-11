﻿using ID.Domain.Entities.Teams;
using ID.Infrastructure.Persistance.Abstractions.Repos.Specs;
using Microsoft.EntityFrameworkCore;

namespace ID.Infrastructure.Persistance.EF.Repos.Specs.Teams;
/// <summary>
/// Specification for querying a Team entity by its ID, including its subscriptions and members up to a specified position.
/// </summary>
internal class TeamByIdWithMembersSpec : GetByIdSpec<Team>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TeamByIdWithMembersSpec"/> class.
    /// </summary>
    /// <param name="teamId">The ID of the team to query.</param>
    /// <param name="maxPosition">The maximum position of members to include in the query.</param>
    public TeamByIdWithMembersSpec(Guid? teamId, int maxPosition) : base(teamId)
    {
        SetInclude(query =>
            query.Include(t => t.Subscriptions)
                    .ThenInclude(t => t.Devices)
            .Include(b => b.Members.Where(m => m.TeamPosition <= maxPosition))
        );
    }

}//Cls
