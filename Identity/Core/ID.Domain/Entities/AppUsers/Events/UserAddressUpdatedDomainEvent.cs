using ID.Domain.Abstractions.Events;
using ID.Domain.Entities.AppUsers;

namespace ID.Domain.Entities.AppUsers.Events;
public sealed record UserAddressUpdatedDomainEvent(AppUser User, IdentityAddress? Address) : IIdDomainEvent  //? Because it may have been deleted.
{
    
}
