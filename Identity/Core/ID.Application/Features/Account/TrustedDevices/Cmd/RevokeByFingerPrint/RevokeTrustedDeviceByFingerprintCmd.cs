namespace ID.Application.Features.Account.TrustedDevices.Cmd.RevokeByFingerPrint;

public sealed record RevokeTrustedDeviceByFingerprintDto(string DeviceFingerprint);

public record RevokeTrustedDeviceByFingerprintCmd(RevokeTrustedDeviceByFingerprintDto Dto) : AIdUserAwareCommand<AppUser>;
