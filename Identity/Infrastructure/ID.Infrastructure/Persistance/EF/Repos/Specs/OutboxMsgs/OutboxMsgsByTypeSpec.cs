using ClArch.SimpleSpecification;
using ID.Domain.Entities.OutboxMessages;

namespace ID.Infrastructure.Persistance.EF.Repos.Specs.OutboxMsgs;

/// <summary>
/// Specification for querying IdOutboxMessages by type.
/// </summary>
internal class OutboxMsgsByTypeSpec : ASimpleSpecification<IdOutboxMessage>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OutboxMsgsByTypeSpec"/> class.
    /// </summary>
    /// <param name="type">The type to query by.</param>
    public OutboxMsgsByTypeSpec(string? type)
        : base(e => e.Type.Contains(type!))
    {
        SetShortCircuit(() => string.IsNullOrWhiteSpace(type));
    }
}
