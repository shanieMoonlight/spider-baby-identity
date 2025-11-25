using ID.Domain.Abstractions.Events;

namespace ID.Domain.Entities.TrustedDevices.Events;
public record TrustedDeviceExpiredDomainEvent(Guid TrustedDeviceId, Guid UserId) : IIdDomainEvent;