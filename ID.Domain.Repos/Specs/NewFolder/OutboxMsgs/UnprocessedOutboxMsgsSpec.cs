using ClArch.SimpleSpecification;
using ID.Domain.Entities.OutboxMessages;

namespace ID.Domain.Repos.Specs.NewFolder.OutboxMsgs;

/// <summary>
/// Specification for all unprocessed outbox messages.
/// </summary>
public class UnprocessedOutboxMsgsSpec : ASimpleSpecification<IdOutboxMessage>
{
    public int? Seed { get; private set; }

    //--------------------------// 


    /// <summary>
    /// Initializes a new instance of the <see cref="UnprocessedOutboxMsgsSpec"/> class.
    /// </summary>
    private UnprocessedOutboxMsgsSpec(int takeCount)
        : base(om => om.ProcessedOnUtc == null)
    {

        Seed = takeCount;
        SetTake(takeCount);
        SetOrderBy(qry => qry.OrderBy(o => o.CreatedOnUtc));
    }

    //--------------------------// 

    public static UnprocessedOutboxMsgsSpec Create(int takeCount = 20) =>
        new(takeCount);


}//Cls
