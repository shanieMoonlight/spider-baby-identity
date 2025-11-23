using ID.Application.Features.Account.TrustedDevices;
using ID.Domain.Abstractions.Services.TrustedDevices;
using ID.Domain.Entities.TrustedDevices.ValueObjects;
using Microsoft.AspNetCore.Http;

namespace ID.Application.Features.Account.TrustedDevices.Cmd.Trust;

public class TrustDeviceCmdHandler(ITrustedDeviceService<AppUser> _service, IHttpContextAccessor httpContextAccessor) : IIdCommandHandler<TrustDeviceCmd, TrustedDeviceDto>
{
    public async Task<GenResult<TrustedDeviceDto>> Handle(TrustDeviceCmd request, CancellationToken cancellationToken)
    {
        var user = request.PrincipalUser;
        var dto = request.Dto;

        // Build ValueObjects
        var fingerprint = DeviceFingerprint.Create(dto.DeviceFingerprint);
        var name = DeviceName.Create(dto.DeviceName);
       
        var userAgent = UserAgent.Create(GetUserAgent());
        var ipAddress = IpAddress.Create(GetIpAddress());

        var addResult =  await _service.AddAsync(
            user:user,
            deviceFingerprint: fingerprint, 
            deviceName: name, 
            userAgent: userAgent,
            ipAddress: ipAddress,
            cancellationToken: cancellationToken);

        if (!addResult.Succeeded)
            return addResult.Convert<TrustedDeviceDto>();

        var newDeviceDto = addResult.Value!.ToDto(); //Success is non-null
        return GenResult<TrustedDeviceDto>.Success(newDeviceDto);
    }

    //---------------------------//

    private  string GetUserAgent()
    {
        var userAgent = httpContextAccessor.HttpContext?.Request.Headers.UserAgent.ToString();
        return string.IsNullOrWhiteSpace(userAgent) 
            ? "Unknown UserAgent" 
            : userAgent;
    }


    //---------------------------//

    private string GetIpAddress()
    {
        var ctx = httpContextAccessor.HttpContext;
        var ip = ctx?.Connection?.RemoteIpAddress;

        if (ip is null)
            return "Unknown IP Address";

        // Normalize IPv4-mapped IPv6 to IPv4
        if (ip.IsIPv4MappedToIPv6) 
            ip = ip.MapToIPv4();

        return ip.ToString();
    }


}//Cls
