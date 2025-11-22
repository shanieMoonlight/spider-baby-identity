using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Abstractions.Services.TrustedDevices;
using ID.Domain.Entities.AppUsers;
using Microsoft.Extensions.Logging;
using MyResults;

namespace ID.Application.Features.Account.Cmd.TrustedDevices.Cmd.Revoke;

public class RevokeTrustedDeviceCmdHandler(ITrustedDeviceService<AppUser> _service, ILogger<RevokeTrustedDeviceCmdHandler> logger)
    : IIdCommandHandler<RevokeTrustedDeviceCmd>
{
    public async Task<BasicResult> Handle(RevokeTrustedDeviceCmd request, CancellationToken cancellationToken)
    {
        var user = request.PrincipalUser;
        return await _service.RevokeAsync(user, request.Dto.DeviceId, cancellationToken);
    }
}
