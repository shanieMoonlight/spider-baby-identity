using ClArch.SimpleSpecification;
using ID.Domain.Entities.TrustedDevices;

namespace ID.Domain.Repos.Specs.TrustedDevices;

internal class TrustedDeviceByUserAndFingerprintSpec : ASimpleSpecification<TrustedDevice>
{
    public TrustedDeviceByUserAndFingerprintSpec(Guid userId, string fingerprint) 
        : base(d => 
        d.UserId == userId 
        && d.DeviceFingerprint == fingerprint)
    {
        SetShortCircuit(() =>
            userId == default
            || string.IsNullOrWhiteSpace(fingerprint));
    }

    public static TrustedDeviceByUserAndFingerprintSpec Create(Guid userId, string fingerprint) => new(userId, fingerprint);
}//Cls
