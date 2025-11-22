namespace ID.Application.Features.Account.Cmd.TrustedDevices.Qry.GetById;
public record GetTrustedDeviceByIdQry(Guid Id) : AIdUserAwareQuery<AppUser, TrustedDeviceDto>;

