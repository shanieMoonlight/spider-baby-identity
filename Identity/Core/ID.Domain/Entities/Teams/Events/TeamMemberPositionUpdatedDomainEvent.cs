using ID.Domain.Abstractions.Events;

namespace ID.Domain.Entities.Teams.Events;
public sealed record TeamMemberPositionUpdatedDomainEvent(Guid TeamId, Guid UserId, int NewPosition) : IIdDomainEvent 
{
    
}
