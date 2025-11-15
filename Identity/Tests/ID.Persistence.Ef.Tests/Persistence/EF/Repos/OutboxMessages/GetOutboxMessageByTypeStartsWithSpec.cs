using ClArch.SimpleSpecification;
using ID.Domain.Entities.OutboxMessages;

namespace ID.Infrastructure.Tests.Persistence.EF.Repos.OutboxMessages;

public class GetOutboxMessageByTypeStartsWithSpec : ASimpleSpecification<IdOutboxMessage> 
{
    internal GetOutboxMessageByTypeStartsWithSpec(string? type) : base(m => m.Type.StartsWith(type!))
    {
        SetShortCircuit(() => string.IsNullOrWhiteSpace(type));
    }
}
