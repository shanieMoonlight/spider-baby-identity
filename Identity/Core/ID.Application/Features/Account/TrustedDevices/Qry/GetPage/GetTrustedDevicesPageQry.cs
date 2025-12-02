using ID.Application.Features.Account.TrustedDevices;
using Pagination;

namespace ID.Application.Features.Account.TrustedDevices.Qry.GetPage;
public record GetTrustedDevicesPageQry(PagedRequest? PagedRequest) : AIdUserAwareQuery<AppUser, PagedResponse<TrustedDeviceDto>>;