using ID.Application.Features.Teams;
using ID.Application.Mediatr.Cqrslmps.Queries;
using Pagination;

namespace ID.Application.Features.Teams.Qry.GetPage;

public record GetTeamsPageQry(PagedRequest? PagedRequest) : AIdQuery<PagedResponse<TeamDto>>;


