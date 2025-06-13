using ID.Domain.Abstractions.Events;
using ID.Domain.Entities.Teams;

namespace ID.Domain.Entities.Teams.Events;
public sealed record TeamCreatedDomainEvent(Guid TeamId, Team Team) : IIdDomainEvent //? because user might have removed a phone number
{
    
}
