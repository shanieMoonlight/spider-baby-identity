using ID.Domain.Abstractions.Events;
using ID.Domain.Entities.AppUsers;
using MassTransit;

namespace ID.Domain.Entities.AppUsers.Events;
public sealed record UserEmailUpdatedDomainEvent(AppUser User) : IIdDomainEvent
{
    
}
