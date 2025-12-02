using ID.Domain.Abstractions.Events;

namespace ID.Domain.Entities.AppUsers.Events;
public sealed record UserPhoneUpdatedDomainEvent(Guid UserId, Guid TeamId, string? Phone) : IIdDomainEvent;
