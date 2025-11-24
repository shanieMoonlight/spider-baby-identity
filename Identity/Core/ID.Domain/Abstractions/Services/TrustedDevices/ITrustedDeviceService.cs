using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.TrustedDevices;
using ID.Domain.Entities.TrustedDevices.ValueObjects;
using MyResults;

namespace ID.Domain.Abstractions.Services.TrustedDevices;

public interface ITrustedDeviceService<TUser>
    where TUser : AppUser
{
    Task<GenResult<TrustedDevice>> AddAsync(
        TUser user,
        DeviceFingerprint deviceFingerprint,
        DeviceName deviceName,
        UserAgent userAgent,
        IpAddress ipAddress,
        CancellationToken cancellationToken
    );

    Task<BasicResult> RevokeAsync(TUser user, Guid deviceId, CancellationToken cancellationToken);

    Task<BasicResult> RevokeAsync(
        TUser user,
        string deviceFingerprint,
        CancellationToken cancellationToken
    );

    //- - - - - - - - - - - - - //
    /// <summary>
    /// Check whether the provided device fingerprint is trusted for the given user.
    /// Operates on the passed-in user aggregate when possible.
    /// </summary>
    Task<bool> IsDeviceTrustedAsync(
        TUser user,
        string deviceFingerprint,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Find a trusted device by fingerprint for the given user (may return expired device).
    /// </summary>
    Task<TrustedDevice?> GetByFingerprintAsync(
        TUser user,
        string deviceFingerprint,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Update the LastUsedDate for a trusted device and persist.
    /// </summary>
    Task UpdateLastUsedAsync(TUser user, TrustedDevice device, CancellationToken cancellationToken);
} //Cls
