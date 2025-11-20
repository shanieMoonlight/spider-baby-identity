using ClArch.SimpleSpecification;
using ID.Domain.Entities.OutboxMessages;

namespace ID.Domain.Repos.Specs.OutboxMsgs;

/// <summary>
/// Specification for all completed outbox messages older than the specified number of days.
/// </summary>
public class OutboxMsgsCompletedOlderThanSpec : ASimpleSpecification<IdOutboxMessage>
{
    public int? Seed { get; private set; }

    //--------------------------// 

    /// <summary>
    /// Initializes a new instance of the <see cref="OutboxMsgsByTypeSpec"/> class.
    /// </summary>
    /// <param name="days">The number of days to look back. Default is 14 days.</param>
    internal OutboxMsgsCompletedOlderThanSpec(int days = 14)
        : base(om =>
            om.CreatedOnUtc < DateTime.Now.AddDays(-days)
            && om.ProcessedOnUtc != null
            && string.IsNullOrWhiteSpace(om.Error)
        )
    {
        Seed = days;
        SetShortCircuit(() => days < 0);
    }


}//Cls
