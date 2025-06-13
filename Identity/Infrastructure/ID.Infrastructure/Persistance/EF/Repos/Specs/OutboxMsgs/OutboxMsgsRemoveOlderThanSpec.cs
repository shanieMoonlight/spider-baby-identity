using ID.Domain.Entities.OutboxMessages;
using ID.Infrastructure.Persistance.Abstractions.Repos.Specs;

namespace ID.Infrastructure.Persistance.EF.Repos.Specs.OutboxMsgs;

/// <summary>
/// Specification for all completed outbox messages older than the specified number of days.
/// </summary>
internal class OutboxMsgsRemoveOlderThanSpec : RemoveRangeSpec<IdOutboxMessage>
{

    /// <summary>
    /// Initializes a new instance of the <see cref="OutboxMsgsRemoveOlderThanSpec"/> class.
    /// </summary>
    /// <param name="days">The number of days to look back. Default is 14 days.</param>
    public OutboxMsgsRemoveOlderThanSpec(int days = 14)
        : base(om =>
            om.CreatedOnUtc < DateTime.Now.AddDays(-days)
            && om.ProcessedOnUtc != null
            && string.IsNullOrWhiteSpace(om.Error)
        )
    {
        SetShortCircuit(() => days < 0);
    }

}
