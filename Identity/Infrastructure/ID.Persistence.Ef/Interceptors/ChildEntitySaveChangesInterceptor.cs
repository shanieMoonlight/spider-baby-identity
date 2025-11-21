//using ID.Domain.Entities.AppUsers;
//using ID.Domain.Entities.Teams;
//using ID.Domain.Entities.TrustedDevices;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Diagnostics;
//using System.Linq;

//namespace ID.Persistence.Ef.Interceptors;

//internal class ChildEntitySaveChangesInterceptor : SaveChangesInterceptor
//{
//    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
//        DbContextEventData eventData,
//        InterceptionResult<int> result,
//        CancellationToken cancellationToken = default)
//    {
//        var ctx = eventData.Context;
//        if (ctx == null) return await base.SavingChangesAsync(eventData, result, cancellationToken);

//        // 1) Ensure child entities that are present in tracked aggregates are tracked (Added) so EF will INSERT them.
//        // AppUser -> TrustedDevice
//        var userEntries = ctx.ChangeTracker.Entries<AppUser>()
//            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
//            .Select(e => e.Entity)
//            .ToList();

//        foreach (var user in userEntries)
//        {
//            foreach (var d in user.TrustedDevices)
//            {
//                var tracked = ctx.ChangeTracker.Entries<TrustedDevice>().Any(x => ReferenceEquals(x.Entity, d));
//                if (!tracked)
//                    ctx.Add(d);
//            }
//        }

//        // Team -> TeamSubscription -> TeamDevice
//        var teamEntries = ctx.ChangeTracker.Entries<Team>()
//            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
//            .Select(e => e.Entity)
//            .ToList();

//        foreach (var team in teamEntries)
//        {
//            foreach (var sub in team.Subscriptions)
//            {
//                var trackedSub = ctx.ChangeTracker.Entries<TeamSubscription>().Any(x => ReferenceEquals(x.Entity, sub));
//                if (!trackedSub)
//                    ctx.Add(sub);

//                foreach (var dvc in sub.Devices)
//                {
//                    var trackedDvc = ctx.ChangeTracker.Entries<TeamDevice>().Any(x => ReferenceEquals(x.Entity, dvc));
//                    if (!trackedDvc)
//                        ctx.Add(dvc);
//                }
//            }
//        }

//        // 2) Handle cases where child is tracked but EF marked it Modified and it doesn't exist in DB (treat as Added)
//        // TrustedDevice
//        var tdEntries = ctx.ChangeTracker.Entries<TrustedDevice>().Where(e => e.State == EntityState.Modified).ToList();
//        if (tdEntries.Count > 0)
//        {
//            var ids = tdEntries.Select(e => e.Entity.Id).ToList();
//            var exists = await ctx.Set<TrustedDevice>().Where(d => ids.Contains(d.Id)).Select(d => d.Id).ToHashSetAsync(cancellationToken);
//            foreach (var e in tdEntries)
//            {
//                if (!exists.Contains(e.Entity.Id))
//                    e.State = EntityState.Added;
//            }
//        }

//        // TeamSubscription
//        var tsEntries = ctx.ChangeTracker.Entries<TeamSubscription>().Where(e => e.State == EntityState.Modified).ToList();
//        if (tsEntries.Count > 0)
//        {
//            var ids = tsEntries.Select(e => e.Entity.Id).ToList();
//            var exists = await ctx.Set<TeamSubscription>().Where(d => ids.Contains(d.Id)).Select(d => d.Id).ToHashSetAsync(cancellationToken);
//            foreach (var e in tsEntries)
//            {
//                if (!exists.Contains(e.Entity.Id))
//                    e.State = EntityState.Added;
//            }
//        }

//        // TeamDevice
//        var tdvcEntries = ctx.ChangeTracker.Entries<TeamDevice>().Where(e => e.State == EntityState.Modified).ToList();
//        if (tdvcEntries.Count > 0)
//        {
//            var ids = tdvcEntries.Select(e => e.Entity.Id).ToList();
//            var exists = await ctx.Set<TeamDevice>().Where(d => ids.Contains(d.Id)).Select(d => d.Id).ToHashSetAsync(cancellationToken);
//            foreach (var e in tdvcEntries)
//            {
//                if (!exists.Contains(e.Entity.Id))
//                    e.State = EntityState.Added;
//            }
//        }

//        return await base.SavingChangesAsync(eventData, result, cancellationToken);
//    }
//}
