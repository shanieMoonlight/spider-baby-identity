using ID.Application.Features.Common.Dtos.User;
using ID.Application.Dtos.User;
using MyResults;
using ID.Application.AppAbs.Permissions;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Features.MemberMgmt.Qry.GetTeamMemberQry;

internal class GetTeamMemberQryHandler(IAppPermissionService<AppUser> _appPermissions) : IIdQueryHandler<GetTeamMemberQry, AppUserDto>
{

    public async Task<GenResult<AppUserDto>> Handle(GetTeamMemberQry request, CancellationToken cancellationToken)
    {
        Guid memberTeamId = request.TeamId;
        Guid memberId = request.MemberId;
        var canViewUserResult = await _appPermissions.ViewTeamMemberPermissions
            .CanViewTeamMemberAsync(memberTeamId, memberId, request);



        return canViewUserResult.Convert(u => u?.ToDto());
    }

}//Cls

