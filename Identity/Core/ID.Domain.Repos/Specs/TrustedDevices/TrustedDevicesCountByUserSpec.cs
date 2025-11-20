using ClArch.SimpleSpecification;
using ID.Domain.Entities.TrustedDevices;

namespace ID.Domain.Repos.Specs.TrustedDevices;

internal class TrustedDevicesCountByUserSpec : ASimpleSpecification<TrustedDevice>
{
    public TrustedDevicesCountByUserSpec(Guid userId) 
        : base(d => 
        d.UserId == userId 
        && (d.TrustedUntil == null || d.TrustedUntil > DateTime.UtcNow))
    {
        SetShortCircuit(() => userId == default);
    }

    public static TrustedDevicesCountByUserSpec Create(Guid userId) => new(userId);
}
