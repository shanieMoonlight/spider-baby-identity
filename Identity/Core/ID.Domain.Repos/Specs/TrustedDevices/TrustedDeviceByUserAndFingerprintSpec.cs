using ClArch.SimpleSpecification;
using ID.Domain.Entities.TrustedDevices;

namespace ID.Domain.Repos.Specs.TrustedDevices;

internal class TrustedDeviceByUserAndFingerprintSpec : ASimpleSpecification<TrustedDevice>
{
    //For testing purposes
    public Guid SeedUserId { get; set; }
    public string SeedFingerprint { get; set; }


    public TrustedDeviceByUserAndFingerprintSpec(Guid userId, string fingerprint) 
        : base(d => 
        d.UserId == userId 
        && d.Fingerprint == fingerprint)
    {
        SetShortCircuit(() =>
            userId == default
            || string.IsNullOrWhiteSpace(fingerprint));


        SeedFingerprint = fingerprint;
        SeedUserId = userId;

    }

    //-------------------------//

    public static TrustedDeviceByUserAndFingerprintSpec Create(Guid userId, string fingerprint) => new(userId, fingerprint);
}//Cls
