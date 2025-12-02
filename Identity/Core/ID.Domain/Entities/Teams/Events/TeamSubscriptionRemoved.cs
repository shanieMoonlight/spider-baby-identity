using ID.Domain.Abstractions.Events;

namespace ID.Domain.Entities.Teams.Events;

public record TeamSubscriptionRemovedEvent(Guid TeamId, Guid SubscriptionId) : IIdDomainEvent;