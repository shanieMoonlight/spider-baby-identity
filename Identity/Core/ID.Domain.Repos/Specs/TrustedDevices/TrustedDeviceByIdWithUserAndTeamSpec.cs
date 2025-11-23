using ClArch.SimpleSpecification;
using ID.Domain.Entities.TrustedDevices;
using Microsoft.EntityFrameworkCore;

namespace ID.Domain.Repos.Specs.TrustedDevices;

internal class TrustedDeviceByIdWithUserAndTeamSpec : ASimpleSpecification<TrustedDevice>
{
    //For testing purposes
    public Guid Seed { get; set; }

    public TrustedDeviceByIdWithUserAndTeamSpec(Guid deviceId) : base(d => d.Id == deviceId)
    {
        SetShortCircuit(() => deviceId == default);
        SetInclude(qry => qry
                .Include(td => td.User)
                    .ThenInclude(user => user!.Team)); //! becuase Devices always have a user
        Seed = deviceId;
    }

    //-------------------------//

    public static TrustedDeviceByIdWithUserAndTeamSpec Create(Guid deviceId) => new(deviceId);

}//Cls
