using ID.Domain.Abstractions.Services.TrustedDevices;
using ID.Domain.Entities.TrustedDevices.ValueObjects;

namespace ID.Application.Features.Account.Cmd.TrustedDevices.Cmd.Trust;

public class TrustDeviceCmdHandler(ITrustedDeviceService<AppUser> _service) : IIdCommandHandler<TrustDeviceCmd, TrustedDeviceDto>
{
    public async Task<GenResult<TrustedDeviceDto>> Handle(TrustDeviceCmd request, CancellationToken cancellationToken)
    {
        var user = request.PrincipalUser;
        var dto = request.Dto;

        // Build ValueObjects
        var fingerprint = DeviceFingerprint.Create(dto.DeviceFingerprint);
        var name = DeviceName.Create(dto.DeviceName);
       
        var userAgent = UserAgent.CreateNullable(GetUserAgent(request));

        var addResult =  await _service.AddAsync(
            user:user,
            deviceFingerprint: fingerprint, 
            deviceName: name, 
            userAgent: userAgent, 
            cancellationToken: cancellationToken);

        if (!addResult.Succeeded)
            return addResult.Convert<TrustedDeviceDto>();

        var newDeviceDto = addResult.Value!.ToDto(); //Success is non-null
        return GenResult<TrustedDeviceDto>.Success(newDeviceDto);
    }

    //---------------------------//

    private static string GetUserAgent(TrustDeviceCmd request)
    {
        var dto = request.Dto;
        //var safeUserAgent = dto.UserAgent;
        if (!string.IsNullOrWhiteSpace(dto.UserAgent))
            return dto.UserAgent;

        return string.IsNullOrWhiteSpace(request.UserAgent) 
            ? "Unknown UserAgent" 
            : request.UserAgent;
    }
}
