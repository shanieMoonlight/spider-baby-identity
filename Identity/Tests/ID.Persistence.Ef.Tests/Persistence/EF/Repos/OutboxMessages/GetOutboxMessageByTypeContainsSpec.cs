using ClArch.SimpleSpecification;
using ID.Domain.Entities.OutboxMessages;

namespace ID.Infrastructure.Tests.Persistence.EF.Repos.OutboxMessages;

public class GetOutboxMessageByTypeContainsSpec : ASimpleSpecification<IdOutboxMessage> 
{
    internal GetOutboxMessageByTypeContainsSpec(string type) : base(m => m.Type.Contains(type))
    {
        SetShortCircuit(() => string.IsNullOrWhiteSpace(type));
    }
}
