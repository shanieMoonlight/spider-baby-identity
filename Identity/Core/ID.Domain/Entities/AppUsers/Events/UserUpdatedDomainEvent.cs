using ID.Domain.Abstractions.Events;
using ID.Domain.Entities.AppUsers;
using MassTransit;

namespace ID.Domain.Entities.AppUsers.Events;
public sealed record UserUpdatedDomainEvent(Guid UserId, AppUser User) : IIdDomainEvent //? because user might have removed a phone number
{
    
}
