using ID.Domain.Abstractions.Events;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Models;

namespace ID.Domain.Entities.AppUsers.Events;
public sealed record User2FactorUpdatedDomainEvent(AppUser User, TwoFactorProvider? Provider) : IIdDomainEvent
{
    
}
