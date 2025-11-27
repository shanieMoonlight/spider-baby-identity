using ID.Domain.Abstractions.Events;

namespace ID.Domain.Entities.AppUsers.Events;
public sealed record User2FactorEnableChangedDomainEvent(Guid UserId, Guid TeamId, bool Enabled) : IIdDomainEvent;
