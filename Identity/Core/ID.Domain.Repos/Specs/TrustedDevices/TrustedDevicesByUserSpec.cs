using ClArch.SimpleSpecification;
using ID.Domain.Entities.TrustedDevices;

namespace ID.Domain.Repos.Specs.TrustedDevices;

internal class TrustedDevicesByUserSpec : ASimpleSpecification<TrustedDevice>
{
    public TrustedDevicesByUserSpec(Guid userId) : base(d => d.UserId == userId)
    {
        SetShortCircuit(() => userId == default);
    }

    public static TrustedDevicesByUserSpec Create(Guid userId) => new(userId);
}//Cls
