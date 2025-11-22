using Pagination;

namespace ID.Application.Features.Account.Cmd.TrustedDevices.Qry.GetPage;
public record GetTrustedDevicesPageQry(PagedRequest? PagedRequest) : AIdUserAwareQuery<AppUser, PagedResponse<TrustedDeviceDto>>;