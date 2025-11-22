namespace ID.Application.Features.Account.Cmd.TrustedDevices.Qry.GetByName;
public record class GetTrustedDeviceByFingerprintQry(string DeviceFingerprint) : AIdUserAwareQuery<AppUser, TrustedDeviceDto>;

