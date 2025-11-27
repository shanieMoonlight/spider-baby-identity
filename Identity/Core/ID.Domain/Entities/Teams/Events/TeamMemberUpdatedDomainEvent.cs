using ID.Domain.Abstractions.Events;

namespace ID.Domain.Entities.Teams.Events;
public sealed record TeamMemberUpdatedDomainEvent(Guid TeamId, Guid UserId) : IIdDomainEvent;
