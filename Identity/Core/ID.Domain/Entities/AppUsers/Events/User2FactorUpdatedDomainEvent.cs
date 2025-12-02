using ID.Domain.Abstractions.Events;
using ID.Domain.Models;

namespace ID.Domain.Entities.AppUsers.Events;
public sealed record User2FactorProviderUpdatedDomainEvent(Guid UserId, Guid TeamId, TwoFactorProvider? Provider) : IIdDomainEvent;