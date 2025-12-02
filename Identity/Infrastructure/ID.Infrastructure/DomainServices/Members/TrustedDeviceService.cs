using ID.Domain.Abstractions.Services.TrustedDevices;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.TrustedDevices;
using ID.Domain.Entities.TrustedDevices.ValueObjects;
using ID.Domain.Repos;
using ID.Domain.Utility.Messages;
using MyResults;
using ID.Domain.Entities.AppUsers.Validators;
using ID.GlobalSettings.Setup.Options;
using Microsoft.Extensions.Options;


namespace ID.Infrastructure.DomainServices.Members;
internal class TrustedDeviceService<TUser>( IIdUnitOfWork _uow, IOptions<IdGlobalOptions> _optsProvider) 
    : ITrustedDeviceService<TUser> where TUser : AppUser
{

    private readonly IdGlobalOptions _options = _optsProvider.Value;

    //-----------------------------//

    public async Task<GenResult<TrustedDevice>> AddAsync(
        TUser user,
        DeviceFingerprint deviceFingerprint,
        DeviceName deviceName,
        UserAgent userAgent,
        IpAddress ipAddress,
        CancellationToken cancellationToken)
    {
        TrustDuration trustDuration = TrustDuration.Create(_options.TrustedDeviceExpireTimeSpan);

        var validation = TrustedDeviceValidators.Addition.Validate(
            user, 
            deviceFingerprint, 
            deviceName, 
            userAgent, 
            ipAddress, 
            trustDuration);

        if (!validation.Succeeded)
            return validation.Convert<TrustedDevice>();

        var device = user.TrustDevice(validation.Value!);

        await _uow.SaveChangesAsync(cancellationToken);


        return GenResult<TrustedDevice>.Success(device);
    }

    //-----------------------//


    public async Task<BasicResult> RevokeAsync(TUser user, Guid deviceId, CancellationToken cancellationToken)
    {
        var device = user.FindTrustedDevice(deviceId);
        if (device is null)
            return BasicResult.NotFoundResult(IDMsgs.Error.NotFound<TrustedDevice>(deviceId));

        var validationResult = TrustedDeviceValidators.Revocation.Validate(user, device);
        if (!validationResult.Succeeded)
            return validationResult.ToBasicResult();

        var revoked = user.RevokeTrustedDevice(validationResult.Value!);
        if (!revoked)
            return BasicResult.BadRequestResult(IDMsgs.Error.TrustedDevices.ALREADY_REVOKED(device, user));

        await _uow.SaveChangesAsync(cancellationToken);

        return BasicResult.Success(IDMsgs.Info.TrustedDevices.REVOKED(device, user));
    }


    //-----------------------//


    public async Task<BasicResult> RevokeAsync(TUser user, string deviceFingerprint, CancellationToken cancellationToken)
    {
        var first = user.TrustedDevices.FirstOrDefault();

        var device = user.TrustedDevices.FirstOrDefault(dvc => dvc.Fingerprint == deviceFingerprint);
        if (device is null)
            return BasicResult.NotFoundResult(IDMsgs.Error.NotFound<TrustedDevice>(deviceFingerprint));

        var validationResult = TrustedDeviceValidators.Revocation.Validate(user, device);
        if (!validationResult.Succeeded)
            return validationResult.ToBasicResult();

        var revoked = user.RevokeTrustedDevice(validationResult.Value!);
        if (!revoked)
            return BasicResult.BadRequestResult(IDMsgs.Error.TrustedDevices.ALREADY_REVOKED(device, user));

        await _uow.SaveChangesAsync(cancellationToken);

        return BasicResult.Success(IDMsgs.Info.TrustedDevices.REVOKED(device, user));
    }


    //-----------------------//

    public Task<bool> IsDeviceTrustedAsync(TUser user, string deviceFingerprint, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(deviceFingerprint))
            return Task.FromResult(false);

        var device = user.TrustedDevices.FirstOrDefault(d => d.Fingerprint == deviceFingerprint && !d.IsExpired());
        return Task.FromResult(device is not null);
    }

    //-----------------------//

    public Task<TrustedDevice?> GetByFingerprintAsync(TUser user, string deviceFingerprint, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(deviceFingerprint))
            return Task.FromResult<TrustedDevice?>(null);

        var device = user.TrustedDevices.FirstOrDefault(d => d.Fingerprint == deviceFingerprint);
        return Task.FromResult(device);
    }

    //-----------------------//
    
    public async Task UpdateLastUsedAsync(TUser user, TrustedDevice device, CancellationToken cancellationToken)
    {
        if (device is null) return;

        device.UpdateLastUsed();
        await _uow.SaveChangesAsync(cancellationToken);
    }

}//Cls
