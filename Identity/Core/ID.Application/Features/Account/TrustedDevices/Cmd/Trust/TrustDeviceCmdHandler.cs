using ID.Application.AppAbs.TrustedDevices;
using Microsoft.AspNetCore.Http;

namespace ID.Application.Features.Account.TrustedDevices.Cmd.Trust;

public class TrustDeviceCmdHandler(IDeviceTrustService<AppUser> _deviceTrustService) : IIdCommandHandler<TrustDeviceCmd, TrustedDeviceDto>
{
    public async Task<GenResult<TrustedDeviceDto>> Handle(TrustDeviceCmd request, CancellationToken cancellationToken)
    {
        var user = request.PrincipalUser;
        var dto = request.Dto;

        var addResult = await _deviceTrustService.TrustAsync(
            user: user,
            deviceFingerprint: dto.DeviceFingerprint,
            deviceName: dto.DeviceName,
            cancellationToken: cancellationToken);

        if (!addResult.Succeeded)
            return addResult.Convert<TrustedDeviceDto>();

        return GenResult<TrustedDeviceDto>.Success(addResult.Value!);
    }

    ////---------------------------//

    //private  string GetUserAgent()
    //{
    //    var userAgent = httpContextAccessor.HttpContext?.Request.Headers.UserAgent.ToString();
    //    return string.IsNullOrWhiteSpace(userAgent) 
    //        ? "Unknown UserAgent" 
    //        : userAgent;
    //}


    ////---------------------------//

    //private string GetIpAddress()
    //{
    //    var ctx = httpContextAccessor.HttpContext;
    //    var ip = ctx?.Connection?.RemoteIpAddress;

    //    if (ip is null)
    //        return "Unknown IP Address";

    //    // Normalize IPv4-mapped IPv6 to IPv4
    //    if (ip.IsIPv4MappedToIPv6) 
    //        ip = ip.MapToIPv4();

    //    return ip.ToString();
    //}


}//Cls
