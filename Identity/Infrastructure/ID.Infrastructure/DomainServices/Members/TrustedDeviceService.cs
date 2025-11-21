using ID.Domain.Abstractions.Services.TrustedDevices;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.TrustedDevices;
using ID.Domain.Entities.TrustedDevices.ValueObjects;
using ID.Domain.Repos;
using ID.Domain.Utility.Messages;
using MyResults;
using ID.Domain.Entities.AppUsers.Validators;


namespace ID.Infrastructure.DomainServices.Members;
internal class TrustedDeviceService<TUser>(IIdUnitOfWork uow) : ITrustedDeviceService<TUser> where TUser : AppUser
{

    //-----------------------//

    public async Task<GenResult<TrustedDevice>> AddAsync(
        TUser user,
        DeviceFingerprint deviceFingerprint,
        DeviceName deviceName,
        UserAgent userAgent,
        CancellationToken cancellationToken)
    {
        TrustDurationNullable trustDuration = TrustDurationNullable.Create(null);

        var validation = TrustedDeviceValidators.Addition.Validate(user, deviceFingerprint, deviceName, userAgent, trustDuration);
        if (!validation.Succeeded)
            return validation.Convert<TrustedDevice>();

        // Apply to aggregate
        var device = user.TrustDevice(validation.Value!);

        await uow.SaveChangesAsync(cancellationToken);


        return GenResult<TrustedDevice>.Success(device);
    }


    //-----------------------//


    public async Task<BasicResult> RevokeAsync(TUser user, Guid deviceId, CancellationToken cancellationToken)
    {
        var first = user.TrustedDevices.FirstOrDefault();

        var device = user.FindTrustedDevice(deviceId);
        if (device is null)
            return BasicResult.NotFoundResult(IDMsgs.Error.NotFound<TrustedDevice>(deviceId));

        var validation = TrustedDeviceValidators.Revocation.Validate(user, device);
        if (!validation.Succeeded)
            return BasicResult.BadRequestResult(validation.Info);

        var revoked = user.RevokeTrustedDevice(validation.Value!);
        if (!revoked)
            return BasicResult.BadRequestResult(IDMsgs.Error.TrustedDevices.ALREADY_REVOKED(device, user));

        await uow.SaveChangesAsync(cancellationToken);

        return BasicResult.Success(IDMsgs.Info.TrustedDevices.REVOKED(device, user));
    }


    //-----------------------//


    public async Task<BasicResult> RevokeAsync(TUser user, string deviceFingerprint, CancellationToken cancellationToken)
    {
        var first = user.TrustedDevices.FirstOrDefault();

        var device = user.TrustedDevices.FirstOrDefault(dvc => dvc.DeviceFingerprint == deviceFingerprint);
        if (device is null)
            return BasicResult.NotFoundResult(IDMsgs.Error.NotFound<TrustedDevice>(deviceFingerprint));

        var validation = TrustedDeviceValidators.Revocation.Validate(user, device);
        if (!validation.Succeeded)
            return BasicResult.BadRequestResult(validation.Info);

        var revoked = user.RevokeTrustedDevice(validation.Value!);
        if (!revoked)
            return BasicResult.BadRequestResult(IDMsgs.Error.TrustedDevices.ALREADY_REVOKED(device, user));

        await uow.SaveChangesAsync(cancellationToken);

        return BasicResult.Success(IDMsgs.Info.TrustedDevices.REVOKED(device, user));
    }

}
