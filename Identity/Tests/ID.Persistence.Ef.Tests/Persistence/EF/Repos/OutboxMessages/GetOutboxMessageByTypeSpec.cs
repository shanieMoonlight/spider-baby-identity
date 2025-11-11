using ClArch.SimpleSpecification;
using ID.Domain.Entities.OutboxMessages;

namespace ID.Infrastructure.Tests.Persistence.EF.Repos.OutboxMessages;

public class GetOutboxMessageByTypeSpec : ASimpleSpecification<IdOutboxMessage> 
{
    internal GetOutboxMessageByTypeSpec(string? type) : base(m => m.Type == type)
    {
        SetShortCircuit(() => string.IsNullOrWhiteSpace(type));
    }
}
