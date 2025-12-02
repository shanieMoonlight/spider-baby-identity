using ID.Application.Features.Account.TrustedDevices;

namespace ID.Application.Features.Account.TrustedDevices.Cmd.Trust;

public sealed record TrustDeviceCreateDto(
    string DeviceFingerprint,
    string DeviceName);


public sealed record TrustDeviceCreateResponseDto(
    TrustedDeviceDto Device,
    string RefreshToken);


public record TrustDeviceCmd(TrustDeviceCreateDto Dto) : AIdUserAwareCommand<AppUser, TrustDeviceCreateResponseDto>;
