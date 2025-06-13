using ID.Domain.Abstractions.Events;

namespace ID.Domain.Entities.Teams.Events;

public sealed record TeamPositionRangeUpdatedDomainEvent(Guid TeamId, int Min, int Max) : IIdDomainEvent 
{
    
}


