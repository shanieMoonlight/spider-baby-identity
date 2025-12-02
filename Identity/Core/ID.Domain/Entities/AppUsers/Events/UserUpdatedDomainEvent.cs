using ID.Domain.Abstractions.Events;

namespace ID.Domain.Entities.AppUsers.Events;
public sealed record UserUpdatedDomainEvent(Guid UserId) : IIdDomainEvent;