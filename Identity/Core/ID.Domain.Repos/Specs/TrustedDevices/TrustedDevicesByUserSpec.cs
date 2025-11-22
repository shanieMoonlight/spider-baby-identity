using ClArch.SimpleSpecification;
using ID.Domain.Entities.TrustedDevices;

namespace ID.Domain.Repos.Specs.TrustedDevices;

internal class TrustedDevicesByUserSpec : ASimpleSpecification<TrustedDevice>
{
    //For testing purposes
    public Guid Seed { get; set; }

    public TrustedDevicesByUserSpec(Guid userId) : base(d => d.UserId == userId)
    {
        SetShortCircuit(() => userId == default);
        Seed = userId;
    }

    //-------------------------//

    public static TrustedDevicesByUserSpec Create(Guid userId) => new(userId);

}//Cls
