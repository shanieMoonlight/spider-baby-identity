using ID.Domain.Abstractions.Events;

namespace ID.Domain.Entities.AppUsers.Events;
public sealed record UserAddressUpdatedDomainEvent(Guid UserId, IdentityAddress? Address) : IIdDomainEvent;
// Address is an owned property of AppUser so has no ID