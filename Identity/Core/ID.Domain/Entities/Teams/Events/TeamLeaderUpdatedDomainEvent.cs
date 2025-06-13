using ID.Domain.Abstractions.Events;

namespace ID.Domain.Entities.Teams.Events;
public sealed record TeamLeaderUpdatedDomainEvent(Guid TeamId, Team Team, Guid NewLeaderId, Guid? OldLeaderId) : IIdDomainEvent 
{
    
}


