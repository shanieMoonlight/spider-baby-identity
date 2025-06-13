using ID.Domain.Entities.Teams;
using ID.Infrastructure.Persistance.Abstractions.Repos.Specs;
using Microsoft.EntityFrameworkCore;

namespace ID.Infrastructure.Persistance.EF.Repos.Specs.Teams;

/// <summary>
/// Specification for retrieving a Team entity by its ID, including its subscriptions, devices, and subscription plans.
/// </summary>
internal class TeamByIdWithSubscriptionsSpec : GetByIdSpec<Team>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TeamByIdWithSubscriptionsSpec"/> class.
    /// </summary>
    /// <param name="teamId">The ID of the team to retrieve.</param>
    public TeamByIdWithSubscriptionsSpec(Guid? teamId) : base(teamId)
    {
        SetInclude(query =>
            query.Include(e => e.Subscriptions)
                    .ThenInclude(t => t.Devices)
                .Include(t => t.Subscriptions)
                    .ThenInclude(t => t.SubscriptionPlan)
        );
    }

}//Cls
