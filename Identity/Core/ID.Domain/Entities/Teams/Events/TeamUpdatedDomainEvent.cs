using ID.Domain.Abstractions.Events;
using ID.Domain.Entities.Teams;

namespace ID.Domain.Entities.Teams.Events;
public sealed record TeamUpdatedDomainEvent(Guid TeamId, Team Team) : IIdDomainEvent //? because user might have removed a phone number
{
    
}
