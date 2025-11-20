using ClArch.SimpleSpecification;
using ID.Domain.Entities.TrustedDevices;

namespace ID.Domain.Repos.Specs.TrustedDevices;

internal class TrustedDevicesExpiredSpec()
    : ASimpleSpecification<TrustedDevice>(d => d.TrustedUntil != null && d.TrustedUntil <= DateTime.UtcNow)
{
    public static TrustedDevicesExpiredSpec Create() => new();
}//Cls
