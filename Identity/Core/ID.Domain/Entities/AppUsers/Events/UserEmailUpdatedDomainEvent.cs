using ID.Domain.Abstractions.Events;

namespace ID.Domain.Entities.AppUsers.Events;
public sealed record UserEmailUpdatedDomainEvent(Guid UserId, Guid TeamId, string NewEmail) : IIdDomainEvent;
