namespace ID.Application.Features.Account.Cmd.TrustedDevices.Cmd.Revoke;

public sealed record RevokeTrustedDeviceDto(Guid DeviceId);

public record RevokeTrustedDeviceCmd(RevokeTrustedDeviceDto Dto) : AIdUserAwareCommand<AppUser>;
