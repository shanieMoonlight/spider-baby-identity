using ID.Application.Features.Common.Dtos.User;
using ID.Application.Dtos.User;
using MyResults;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Abstractions.Services.Teams;

namespace ID.Application.Features.Teams.Qry.GetSprMembers;
internal class GetSprMembersQryHandler(IIdentityTeamManager<AppUser> _mgr) : IIdQueryHandler<GetSprMembersQry, IEnumerable<AppUserDto>>
{
    public async Task<GenResult<IEnumerable<AppUserDto>>> Handle(GetSprMembersQry request, CancellationToken cancellationToken)
    {
        var position = request.IsSuper ? 1000 : request.PrincipalTeamPosition;
        var team = await _mgr.GetSuperTeamWithMembersAsync(position);
        var dtos = team?.Members?.ToDtos() ?? [];

        return GenResult<IEnumerable<AppUserDto>>.Success(dtos);
    }


}//Cls
