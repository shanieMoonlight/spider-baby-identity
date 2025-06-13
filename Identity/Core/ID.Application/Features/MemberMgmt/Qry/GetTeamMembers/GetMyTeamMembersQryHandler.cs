using ID.Application.Features.Common.Dtos.User;
using ID.Application.Dtos.User;
using MyResults;
using ID.Application.Mediatr.CqrsAbs;

namespace ID.Application.Features.MemberMgmt.Qry.GetTeamMembers;

public class GetMyTeamMembersQryHandler() : IIdQueryHandler<GetMyTeamMembersQry, List<AppUserDto>>
{

    public Task<GenResult<List<AppUserDto>>> Handle(GetMyTeamMembersQry request, CancellationToken cancellationToken)
    {
        var team = request.PrincipalTeam;

        var dtos = team?.Members
            .Select(c => c.ToDto())
            .ToList() ?? [];

        return Task.FromResult(GenResult<List<AppUserDto>>.Success(dtos));
    }


}//Cls

