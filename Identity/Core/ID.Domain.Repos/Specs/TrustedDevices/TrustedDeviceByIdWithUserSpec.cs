using ClArch.SimpleSpecification;
using ID.Domain.Entities.TrustedDevices;
using Microsoft.EntityFrameworkCore;

namespace ID.Domain.Repos.Specs.TrustedDevices;

internal class TrustedDeviceByIdWithUserSpec : ASimpleSpecification<TrustedDevice>
{
    //For testing purposes
    public Guid Seed { get; set; }

    public TrustedDeviceByIdWithUserSpec(Guid deviceId) : base(d => d.Id == deviceId)
    {
        SetShortCircuit(() => deviceId == default);
        SetInclude(qry => qry.Include(td => td.User));
        Seed = deviceId;
    }

    //-------------------------//

    public static TrustedDeviceByIdWithUserSpec Create(Guid deviceId) => new(deviceId);

}//Cls
