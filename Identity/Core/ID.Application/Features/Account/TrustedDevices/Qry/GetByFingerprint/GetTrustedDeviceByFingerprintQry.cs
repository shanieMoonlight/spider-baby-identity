using ID.Application.Features.Account.TrustedDevices;

namespace ID.Application.Features.Account.TrustedDevices.Qry.GetByFingerprint;
public record class GetTrustedDeviceByFingerprintQry(string DeviceFingerprint) : AIdUserAwareQuery<AppUser, TrustedDeviceDto>;

