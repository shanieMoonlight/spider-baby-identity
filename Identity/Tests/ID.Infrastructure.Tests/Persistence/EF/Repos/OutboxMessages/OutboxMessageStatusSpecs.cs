using ClArch.SimpleSpecification;
using ID.Domain.Entities.OutboxMessages;

namespace ID.Infrastructure.Tests.Persistence.EF.Repos.OutboxMessages;

public class GetProcessedOutboxMessagesSpec : ASimpleSpecification<IdOutboxMessage> 
{
    internal GetProcessedOutboxMessagesSpec() : base(m => m.ProcessedOnUtc != null)
    {
    }
}

public class GetUnprocessedOutboxMessagesSpec : ASimpleSpecification<IdOutboxMessage> 
{
    internal GetUnprocessedOutboxMessagesSpec() : base(m => m.ProcessedOnUtc == null)
    {
    }
}
