namespace ID.Application.AppAbs.TrustedDevices;

/// <summary>
/// Internal application service to centralize creating trusted-device entries.
/// Not exposed publicly; used by handlers in the application layer.
/// Handles finding the user-agent, IP, etc., from context if needed.
/// </summary>
public interface IDeviceTrustService<TUser>
    where TUser : AppUser
{
    Task<GenResult<TrustedDevice>> TrustAsync(
        TUser user,
        string deviceFingerprint,
        string deviceName,
        CancellationToken cancellationToken = default);
}
