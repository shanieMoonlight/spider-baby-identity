using ID.Domain.Entities.OutboxMessages;
using ID.Infrastructure.Persistance.Abstractions.Repos;
using ID.Infrastructure.Persistance.EF.Repos.Abstractions;
using MyResults;

namespace ID.Infrastructure.Persistance.EF.Repos;
/// <summary>
/// Repository for managing <see cref="IdOutboxMessage"/> entities.
/// </summary>
internal class OutboxMessageRepo(IdDbContext db) : AGenCrudRepo<IdOutboxMessage>(db), IIdentityOutboxMessageRepo
{
    /// <summary>
    /// Determines whether an outbox message can be deleted.
    /// </summary>
    /// <param name="dbMsg">The outbox message to check.</param>
    /// <returns>A result indicating whether the outbox message can be deleted.</returns>
    protected override Task<BasicResult> CanDeleteAsync(IdOutboxMessage? dbMsg)
    {
        if (dbMsg is null)
            return Task.FromResult(BasicResult.Success());

        var result = dbMsg.ProcessedOnUtc is null
            ? BasicResult.BadRequestResult($"Outbox message, {dbMsg.Type} - {dbMsg.CreatedOnUtc} has not been processsed yet so can't be deleted")
            : BasicResult.Success();

        return Task.FromResult(result);
    }

}//Cls
