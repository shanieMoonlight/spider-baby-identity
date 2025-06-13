using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using MyResults;
using Pagination;

namespace ID.Application.Features.Teams.Qry.GetPage;
internal class GetTeamsPageQryHandler(IIdentityTeamManager<AppUser> _mgr) : IIdQueryHandler<GetTeamsPageQry, PagedResponse<TeamDto>>
{
    public async Task<GenResult<PagedResponse<TeamDto>>> Handle(GetTeamsPageQry request, CancellationToken cancellationToken)
    {
        var pgRequest = request.PagedRequest ?? PagedRequest.Empty();

        var page = await _mgr.GetPageAsync(pgRequest);

        var response = new PagedResponse<Team>(page, pgRequest);
        return GenResult<PagedResponse<TeamDto>>.Success(response.Transform(d => d.ToDto()));
    }

}//Cls

