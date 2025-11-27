using ID.Domain.Abstractions.Events;

namespace ID.Domain.Entities.Teams.Events;
public sealed record TeamCreatedDomainEvent(Guid TeamId) : IIdDomainEvent;