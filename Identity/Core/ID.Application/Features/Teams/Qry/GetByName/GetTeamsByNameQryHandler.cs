using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Entities.AppUsers;
using MyResults;

namespace ID.Application.Features.Teams.Qry.GetByName;
internal class GetTeamsByNameQryHandler(IIdentityTeamManager<AppUser> _mgr) : IIdQueryHandler<GetTeamsByNameQry, IEnumerable<TeamDto>>
{
    public async Task<GenResult<IEnumerable<TeamDto>>> Handle(GetTeamsByNameQry request, CancellationToken cancellationToken)
    {
        var name = request.Name;
        var teams = await _mgr.GetCustomerTeamsByNameAsync(name);

        return GenResult<IEnumerable<TeamDto>>.Success(teams.ToDtos());

    }

}//Cls

