using ID.Domain.Abstractions.Events;
using ID.Domain.Entities.AppUsers;

namespace ID.Domain.Entities.AppUsers.Events;
public sealed record User2FactorEnableChangedDomainEvent(AppUser User, bool Enabled) : IIdDomainEvent
{
    
}
