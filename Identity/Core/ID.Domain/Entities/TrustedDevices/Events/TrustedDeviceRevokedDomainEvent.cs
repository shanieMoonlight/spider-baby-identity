using ID.Domain.Abstractions.Events;

namespace ID.Domain.Entities.TrustedDevices.Events;
public sealed class TrustedDeviceRevokedDomainEvent(TrustedDevice trustedDevice, Guid userId) : IIdDomainEvent
{

    public Guid TrustedDeviceId { get; } = trustedDevice.Id;
    public Guid UserId { get; } = userId;
}
