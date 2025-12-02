using ID.Domain.Abstractions.Events;

namespace ID.Domain.Entities.Teams.Events;

public record TeamSubscriptionAddedEvent(Guid TeamId, Guid SubscriptionId) : IIdDomainEvent;

