using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.TrustedDevices;
using ID.Domain.Entities.TrustedDevices.ValueObjects;
using MyResults;

namespace ID.Domain.Abstractions.Services.TrustedDevices;
public interface ITrustedDeviceService<TUser> where TUser : AppUser
{
    Task<GenResult<TrustedDevice>> AddAsync(
        TUser user, 
        DeviceFingerprint deviceFingerprint, 
        DeviceName deviceName, 
        UserAgent userAgent, 
        IpAddress ipAddress,
        CancellationToken cancellationToken);

    Task<BasicResult> RevokeAsync(TUser user, Guid deviceId, CancellationToken cancellationToken);


    Task<BasicResult> RevokeAsync(TUser user, string deviceFingerprint, CancellationToken cancellationToken);

}//Cls