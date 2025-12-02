using ID.Domain.Abstractions.Events;

namespace ID.Domain.Entities.TrustedDevices.Events;
public sealed record TrustedDeviceExtendedDomainEvent(Guid TrustedDeviceId, Guid UserId) : IIdDomainEvent;