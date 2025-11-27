using ID.Domain.Abstractions.Events;

namespace ID.Domain.Entities.Teams.Events;
public record TeamSubscriptionDeactivatedDomainEvent(Guid TeamId, Guid SubscriptionId) : IIdDomainEvent;
//public record TeamSubscriptionDeactivatedDomainEvent(TeamSubscription Subscription) : IIdDomainEvent;