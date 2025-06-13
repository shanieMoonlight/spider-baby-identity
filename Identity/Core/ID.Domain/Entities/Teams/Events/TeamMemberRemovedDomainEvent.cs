using ID.Domain.Abstractions.Events;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;

namespace ID.Domain.Entities.Teams.Events;
public sealed record TeamMemberRemovedDomainEvent(Team Team, AppUser Member) : IIdDomainEvent 
{
    
}
