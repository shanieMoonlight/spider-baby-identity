using MyResults;
using ID.Application.Features.Teams;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Abstractions.Services.Teams;

namespace ID.Application.Features.Teams.Qry.GetAll;
internal class GetAllTeamsQryHandler(IIdentityTeamManager<AppUser> _mgr) : IIdQueryHandler<GetAllTeamsQry, IEnumerable<TeamDto>>
{

    public async Task<GenResult<IEnumerable<TeamDto>>> Handle(GetAllTeamsQry request, CancellationToken cancellationToken)
    {
        var mdls = await _mgr.GetAllTeams(
            includeMntc: request.IsMntcMinimum,
            includeSuper: request.IsSuper,
            cancellationToken);

        var dtos = mdls.Select(mdl => mdl.ToDto());
        return GenResult<IEnumerable<TeamDto>>.Success(dtos);

    }

}//Cls

