using ID.Domain.Abstractions.Events;

namespace ID.Domain.Entities.Teams.Events;
public sealed record TeamMemberAddedDomainEvent(Guid TeamId, Guid UserId) : IIdDomainEvent;
