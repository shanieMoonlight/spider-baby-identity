using ID.Domain.Abstractions.Events;
using ID.Domain.Entities.Teams;

namespace ID.Domain.Entities.Teams.Events;
public record TeamSubscriptionAddedEvent(Team Team, TeamSubscription Subscription) : IIdDomainEvent
{
    
}

