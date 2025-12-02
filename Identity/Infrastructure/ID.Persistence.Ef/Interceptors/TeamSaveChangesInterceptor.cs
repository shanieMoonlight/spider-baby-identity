using ID.Domain.Entities.Teams;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

//Ef Change tracker is having problems finding new Devices and Subscriptions and throwing an error on SaveChanges
//If we make sure to add them first the problem goes away.
//Our entities create their own PRIMARY KEYS so we can identify new vs existing this way.
//So EF can't tell if a Subscription or Device is new or existing when we attach the Team entity for update.
// IT judges-based on whether thePK is default(Guid) or not, but our entities always generate a new Guid on creation.


//NOT WORKING DON'T DELETE YET - TRYING A DIFFERENT APPROACH FIRST, MAY COME BACK TO THIS LATER
internal class TeamSaveChangesInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var ctx = eventData.Context;
        if (ctx == null) return base.SavingChangesAsync(eventData, result, cancellationToken);

        // iterate tracked Team entries
        foreach (var teamEntry in ctx.ChangeTracker.Entries<Team>())
        {
            var team = teamEntry.Entity;
            // only consider teams that will be saved (Added/Modified)
            if (teamEntry.State == EntityState.Unchanged || teamEntry.State == EntityState.Detached) 
                continue;

            // ensure subscription entities are tracked (so EF will INSERT them)
            foreach (var sub in team.Subscriptions)
            {
                var tracked = ctx.ChangeTracker.Entries<TeamSubscription>().Any(e => ReferenceEquals(e.Entity, sub));
                if (!tracked)
                    ctx.Add(sub);

                // optionally ensure devices are tracked too
                foreach (var dvc in sub.Devices)
                {
                    var dvcTracked = ctx.ChangeTracker.Entries<TeamDevice>().Any(e => ReferenceEquals(e.Entity, dvc));
                    if (!dvcTracked)
                        ctx.Add(dvc);
                }
            }
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}