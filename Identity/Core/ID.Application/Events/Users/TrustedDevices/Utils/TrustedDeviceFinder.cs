using ID.Domain.Repos;
using ID.Domain.Repos.Specs.TrustedDevices;

namespace ID.Application.Events.Users.TrustedDevices.Utils;
internal class TrustedDeviceFinder
{

    public static async Task<GenResult<TrustedDevice>> FindWithUserAsync(Guid deviceId, Guid userId, IIdentityTrustedDeviceRepo repo)
    {
        var spec = TrustedDeviceByIdWithUserSpec.Create(deviceId);
        var device = await repo.FirstOrDefaultAsync(spec);

        if (device is null)
            return GenResult<TrustedDevice>.NotFoundResult(IDMsgs.Error.NotFound<TrustedDevice>(deviceId));

        if (device.UserId != userId)
            return GenResult<TrustedDevice>.ForbiddenResult(IDMsgs.Error.TrustedDevices.USER_NOT_OWNER(device, userId));

        return GenResult<TrustedDevice>.Success(device);
    }

}
