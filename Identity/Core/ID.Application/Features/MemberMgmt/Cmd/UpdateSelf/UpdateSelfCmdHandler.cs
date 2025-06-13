using ID.Application.Dtos.User;
using MyResults;
using ID.Application.Features.Common.Dtos.User;
using ID.Application.AppAbs.Permissions;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Abstractions.Services.Teams;

namespace ID.Application.Features.MemberMgmt.Cmd.UpdateSelf;
public class UpdateSelfCmdHandler(IIdentityTeamManager<AppUser> teamMgr, IAppPermissionService<AppUser> _appPermissions)
    : IIdCommandHandler<UpdateSelfCmd, AppUserDto>
{

    public async Task<GenResult<AppUserDto>> Handle(UpdateSelfCmd request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var updateUserId = dto.Id;

        var canUpdateUserResult = await _appPermissions.UpdatePermissions
            .CanUpdateAsync(updateUserId, request);

        if (!canUpdateUserResult.Succeeded)
            return canUpdateUserResult.Convert<AppUserDto>();

        var updateUser = canUpdateUserResult.Value!; //Success is non-null
        var team = request.PrincipalTeam;

        updateUser.Update(dto);

        var updateResult = await teamMgr.UpdateMemberAsync(team, updateUser);

        return updateResult.Convert(u => u?.ToDto());
    }

    //------------------------------------//

}//Cls

