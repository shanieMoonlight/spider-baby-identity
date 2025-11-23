using ID.Application.Features.Account.TrustedDevices;

namespace ID.Application.Features.Account.TrustedDevices.Qry.GetAll;
public record GetAllTrustedDevicesQry : AIdUserAwareQuery<AppUser, IEnumerable<TrustedDeviceDto>>;
