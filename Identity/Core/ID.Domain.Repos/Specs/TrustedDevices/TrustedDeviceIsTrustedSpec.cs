using ClArch.SimpleSpecification;
using ID.Domain.Entities.TrustedDevices;

namespace ID.Domain.Repos.Specs.TrustedDevices;

internal class TrustedDeviceIsTrustedSpec : ASimpleSpecification<TrustedDevice>
{
    //For testing purposes
    public Guid SeedUserId { get; set; }
    public string SeedFingerprint { get; set; }



    public TrustedDeviceIsTrustedSpec(Guid userId, string fingerprint)
        : base(d =>
            d.UserId == userId
            && d.DeviceFingerprint == fingerprint
            && (d.TrustedUntil == null || d.TrustedUntil > DateTime.UtcNow))
    {

        SetShortCircuit(() =>
            userId == default
            || string.IsNullOrWhiteSpace(fingerprint));

        SeedFingerprint = fingerprint;
        SeedUserId = userId;

    }

    //-------------------------//

    public static TrustedDeviceIsTrustedSpec Create(Guid userId, string fingerprint) => new(userId, fingerprint);
}//Cls
