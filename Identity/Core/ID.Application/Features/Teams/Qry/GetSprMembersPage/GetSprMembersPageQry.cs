using ID.Application.Features.Common.Dtos.User;
using ID.Application.Mediatr.Cqrslmps.Queries;
using Pagination;

namespace ID.Application.Features.Teams.Qry.GetSprMembersPage;
public record GetSprMembersPageQry(PagedRequest PagedRequest) : AIdQuery<PagedResponse<AppUserDto>>;
