using ID.Domain.Abstractions.Events;
using ID.Domain.Entities.AppUsers;

namespace ID.Domain.Entities.TrustedDevices.Events;
public sealed class TrustedDeviceAddedDomainEvent(TrustedDevice trustedDevice, AppUser user) : IIdDomainEvent  //? Because it may have been deleted.
{
    public Guid TrustedDeviceId { get; } = trustedDevice.Id;
    public Guid UserId { get; } = user.Id;
}
