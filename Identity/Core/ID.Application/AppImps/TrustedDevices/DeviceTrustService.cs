using ID.Application.AppAbs.TrustedDevices;
using ID.Domain.Abstractions.Services.TrustedDevices;
using ID.Domain.Entities.TrustedDevices.ValueObjects;
using Microsoft.AspNetCore.Http;

namespace ID.Application.AppImps.TrustedDevices;

internal class DeviceTrustService<TUser>(ITrustedDeviceService<TUser> _trustedDeviceService, IHttpContextAccessor _httpContextAccessor) : IDeviceTrustService<TUser>
    where TUser : AppUser
{
    public async Task<GenResult<TrustedDeviceDto>> TrustAsync(
        TUser user,
        string deviceFingerprint,
        string deviceName,
        CancellationToken cancellationToken = default)
    {
        var fingerprint = DeviceFingerprint.Create(deviceFingerprint);
        var name = DeviceName.Create(deviceName);

        string uaValue = GetUserAgent();

        string ipValue = GetIpAddress();

        var ua = UserAgent.Create(uaValue);
        var ip = IpAddress.Create(ipValue);

        var addResult = await _trustedDeviceService.AddAsync(
            user: user,
            deviceFingerprint: fingerprint,
            deviceName: name,
            userAgent: ua,
            ipAddress: ip,
            cancellationToken: cancellationToken);

        if (!addResult.Succeeded)
            return addResult.Convert<TrustedDeviceDto>();

        var dto = addResult.Value!.ToDto();
        return GenResult<TrustedDeviceDto>.Success(dto);
    }

    //---------------------------//

    private string GetUserAgent()
    {
        var userAgent = _httpContextAccessor.HttpContext?.Request.Headers.UserAgent.ToString();
        return string.IsNullOrWhiteSpace(userAgent)
            ? "Unknown UserAgent"
            : userAgent;
    }


    //---------------------------//

    private string GetIpAddress()
    {
        var ctx = _httpContextAccessor.HttpContext;
        var ip = ctx?.Connection?.RemoteIpAddress;

        if (ip is null)
            return "Unknown IP Address";

        // Normalize IPv4-mapped IPv6 to IPv4
        if (ip.IsIPv4MappedToIPv6)
            ip = ip.MapToIPv4();

        return ip.ToString();
    }

}//Cls
