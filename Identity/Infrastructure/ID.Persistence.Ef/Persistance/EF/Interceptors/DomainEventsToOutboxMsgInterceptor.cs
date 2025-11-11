using ID.Domain.Abstractions.Events;
using ID.Domain.Entities.OutboxMessages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ID.Infrastructure.Persistance.EF.Interceptors;
public sealed class DomainEventsToOutboxMsgInterceptor : SaveChangesInterceptor
{

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        DbContext? dbCtx = eventData.Context;
        if (dbCtx == null)
            return await base.SavingChangesAsync(eventData, result, cancellationToken);

        var what = dbCtx.ChangeTracker
            .Entries<IIdDomainEventEntity>()
            .ToList();

        var events = dbCtx.ChangeTracker
                .Entries<IIdDomainEventEntity>()
                .Select(e => e.Entity)
                .SelectMany(bk =>
                {
                    var dEvs = bk.GetDomainEvents();
                    bk.ClearDomainEvents();
                    return dEvs;
                })
                .Select(ev => IdOutboxMessage.Create(ev))
                .ToList();


        if(events.Count > 0)
            await dbCtx.Set<IdOutboxMessage>().AddRangeAsync(events, cancellationToken);


        return await base.SavingChangesAsync(eventData, result, cancellationToken);

    }

}//Cls
