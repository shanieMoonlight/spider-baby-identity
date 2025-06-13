using ID.Application.Features.Common.Dtos.User;
using ID.Application.Mediatr.Cqrslmps.Queries;
using Pagination;

namespace ID.Application.Features.Teams.Qry.GetMntcMembersPage;
public record GetMntcMembersPageQry(PagedRequest PagedRequest) : AIdQuery<PagedResponse<AppUserDto>>;
