using ID.Application.Dtos.User;
using ID.Application.Features.Common.Dtos.User;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Entities.AppUsers;
using MyResults;

namespace ID.Application.Features.Teams.Qry.GetMntcMembers;
public class GetMntcMembersQryHandler(IIdentityTeamManager<AppUser> teamMgr) : IIdQueryHandler<GetMntcMembersQry, IEnumerable<AppUserDto>>
{
    public async Task<GenResult<IEnumerable<AppUserDto>>> Handle(GetMntcMembersQry request, CancellationToken cancellationToken)
    {
        //Super Members see all Mntc Members, Mntc Members see inly their Position and lower
        var teamPosition = request.IsSuperMinimum || request.IsMntcLeader ? 10000 : request.PrincipalTeamPosition;
        var team = await teamMgr.GetMntcTeamWithMembersAsync(teamPosition);

        var dtos = team?.Members?.ToDtos() ?? [];

        return GenResult<IEnumerable<AppUserDto>>.Success(dtos);
    }

}//Cls
