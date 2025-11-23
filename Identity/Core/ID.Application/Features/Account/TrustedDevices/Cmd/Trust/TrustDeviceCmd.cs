using ID.Application.Features.Account.TrustedDevices;

namespace ID.Application.Features.Account.TrustedDevices.Cmd.Trust;

public sealed record TrustDeviceCreateDto(
    string DeviceFingerprint,
    string DeviceName);


public record TrustDeviceCmd(TrustDeviceCreateDto Dto) : AIdUserAwareCommand<AppUser, TrustedDeviceDto>;
