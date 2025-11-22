namespace ID.Application.Features.Account.Cmd.TrustedDevices.Cmd.Trust;

public sealed record TrustDeviceCreateDto(
    string DeviceFingerprint,
    string DeviceName,
    string? UserAgent,
    int? TrustDays);


public record TrustDeviceCmd(TrustDeviceCreateDto Dto, string? UserAgent) : AIdUserAwareCommand<AppUser, TrustedDeviceDto>;
