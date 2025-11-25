using ID.Domain.Abstractions.Services.TrustedDevices;
using Microsoft.Extensions.Logging;

namespace ID.Application.Features.Account.TrustedDevices.Cmd.RevokeByFingerPrint;

public class RevokeTrustedDeviceByFingerPrintCmdHandler(
    ITrustedDeviceService<AppUser> _service,
    ILogger<RevokeTrustedDeviceByFingerPrintCmdHandler> logger
) : IIdCommandHandler<RevokeTrustedDeviceByFingerprintCmd>
{
    public async Task<BasicResult> Handle(
        RevokeTrustedDeviceByFingerprintCmd request,
        CancellationToken cancellationToken
    )
    {
        var user = request.PrincipalUser;
        return await _service.RevokeAsync(user, request.Dto.DeviceFingerprint, cancellationToken);
    }
}
