using ID.Domain.Abstractions.Events;
using ID.Domain.Entities.AppUsers;

namespace ID.Domain.Entities.Teams.Events;
public sealed record TeamMemberAddedDomainEvent(Team Team, AppUser User) : IIdDomainEvent;
