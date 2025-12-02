using ID.Application.AppAbs.TrustedDevices;
using ID.Domain.Repos;
using ID.Domain.Repos.Specs.TrustedDevices;

namespace ID.Application.AppImps.TrustedDevices;
internal class TrustedDeviceFinder(IIdentityTrustedDeviceRepo repo) : ITrustedDeviceFinder
{

    public async Task<GenResult<TrustedDevice>> FindWithUserAndTeamAsync(Guid deviceId, Guid userId)
    {
        var spec = TrustedDeviceByIdWithUserAndTeamSpec.Create(deviceId);
        var device = await repo.FirstOrDefaultAsync(spec);

        if (device is null)
            return GenResult<TrustedDevice>.NotFoundResult(IDMsgs.Error.NotFound<TrustedDevice>(deviceId));

        if (device.UserId != userId)
            return GenResult<TrustedDevice>.ForbiddenResult(IDMsgs.Error.TrustedDevices.USER_NOT_OWNER(device, userId));

        return GenResult<TrustedDevice>.Success(device);
    }

}
