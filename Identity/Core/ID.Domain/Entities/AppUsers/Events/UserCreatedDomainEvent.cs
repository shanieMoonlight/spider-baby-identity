using ID.Domain.Abstractions.Events;

namespace ID.Domain.Entities.AppUsers.Events;
public sealed record UserCreatedDomainEvent(Guid UserId) : IIdDomainEvent;