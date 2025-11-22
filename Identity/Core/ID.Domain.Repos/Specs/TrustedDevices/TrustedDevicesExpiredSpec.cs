using ClArch.SimpleSpecification;
using ID.Domain.Entities.TrustedDevices;

namespace ID.Domain.Repos.Specs.TrustedDevices;

internal class TrustedDevicesExpiredSpec : ASimpleSpecification<TrustedDevice>
{
    //For tests
    internal int Seed { get; set; }


    public TrustedDevicesExpiredSpec(int expiredByDays) : base(d =>
        d.TrustedUntil != null //Null means indefinite trust
        &&
        d.TrustedUntil <= DateTime.UtcNow.AddDays(-expiredByDays))
    {
        Seed = expiredByDays;
        SetShortCircuit(() => Seed < 1);
    }

    public static TrustedDevicesExpiredSpec Create(int expiredByDays) => new(expiredByDays);
}//Cls
