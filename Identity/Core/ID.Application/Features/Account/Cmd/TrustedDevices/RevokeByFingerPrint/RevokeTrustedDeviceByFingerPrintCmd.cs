using ID.Application.Mediatr.Cqrslmps.Commands;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Features.Account.Cmd.TrustedDevices.RevokeByFingerPrint;

public sealed record RevokeTrustedDeviceByFingerprintDto(string DeviceFingerprint);

public record RevokeTrustedDeviceByFingerprintCmd(RevokeTrustedDeviceByFingerprintDto Dto) : AIdUserAwareCommand<AppUser>;
