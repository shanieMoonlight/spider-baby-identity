namespace ID.Application.Features.Account.Cmd.TrustedDevices.Qry.GetAll;
public record GetAllTrustedDevicesQry : AIdUserAwareQuery<AppUser, IEnumerable<TrustedDeviceDto>>;
