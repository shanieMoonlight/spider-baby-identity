using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Abstractions.Services.TrustedDevices;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.TrustedDevices.ValueObjects;
using Microsoft.Extensions.Logging;
using MyResults;

namespace ID.Application.Features.Account.Cmd.TrustedDevices.Trust;

public class TrustDeviceCmdHandler(ITrustedDeviceService<AppUser> _service, ILogger<TrustDeviceCmdHandler> logger) : IIdCommandHandler<TrustDeviceCmd, TrustedDeviceDto>
{
    public async Task<GenResult<TrustedDeviceDto>> Handle(TrustDeviceCmd request, CancellationToken cancellationToken)
    {
        var user = request.PrincipalUser;


        // Build ValueObjects
        var fingerprint = DeviceFingerprint.Create(request.Dto.Fingerprint);
        var name = DeviceName.Create(request.Dto.Name);
        var userAgent = UserAgent.CreateNullable(request.Dto.UserAgent);

        var addResult =  await _service.AddAsync(
            user:user,
            deviceFingerprint: fingerprint, 
            deviceName: name, 
            userAgent: userAgent, 
            cancellationToken: cancellationToken);

        if (!addResult.Succeeded)
            return addResult.Convert<TrustedDeviceDto>();

        var dto = addResult.Value!.ToDto(); //Success is non-null

        return GenResult<TrustedDeviceDto>.Success(dto);
    }
}
