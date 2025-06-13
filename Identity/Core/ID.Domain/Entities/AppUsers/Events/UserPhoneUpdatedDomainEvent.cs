using ID.Domain.Abstractions.Events;

namespace ID.Domain.Entities.AppUsers.Events;
public sealed record UserPhoneUpdatedDomainEvent(AppUser User, string? Phone) : IIdDomainEvent;
