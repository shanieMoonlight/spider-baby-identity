using ID.Application.Features.Account.TrustedDevices;

namespace ID.Application.Features.Account.TrustedDevices.Qry.GetById;
public record GetTrustedDeviceByIdQry(Guid Id) : AIdUserAwareQuery<AppUser, TrustedDeviceDto>;

