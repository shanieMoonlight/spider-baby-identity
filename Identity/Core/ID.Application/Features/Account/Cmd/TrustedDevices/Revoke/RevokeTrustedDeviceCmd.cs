using ID.Application.Mediatr.Cqrslmps.Commands;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Features.Account.Cmd.TrustedDevices.Revoke;

public sealed record RevokeTrustedDeviceDto(Guid DeviceId);

public record RevokeTrustedDeviceCmd(RevokeTrustedDeviceDto Dto) : AIdUserAwareCommand<AppUser>;
