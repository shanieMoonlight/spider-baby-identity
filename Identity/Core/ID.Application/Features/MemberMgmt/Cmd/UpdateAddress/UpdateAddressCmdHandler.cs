using ID.Application.AppAbs.Permissions;
using ID.Application.Dtos.User;
using ID.Application.Features.Common.Dtos.User;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Entities.AppUsers;
using MyResults;

namespace ID.Application.Features.MemberMgmt.Cmd.UpdateAddress;
public class UpdateAddressCmdHandler(IIdentityTeamManager<AppUser> teamMgr, IAppPermissionService<AppUser> _appPermissions)
    : IIdCommandHandler<UpdateAddressCmd, AppUserDto>
{

    public async Task<GenResult<AppUserDto>> Handle(UpdateAddressCmd request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var updateUserId = request.PrincipalUserId!.Value; //Already validated

        var canUpdateUserResult = await _appPermissions.UpdatePermissions
            .CanUpdateAsync(updateUserId, request);

        if (!canUpdateUserResult.Succeeded)
            return canUpdateUserResult.Convert<AppUserDto>();

        var updateUser = canUpdateUserResult.Value!; //Success is non-null
        var team = request.PrincipalTeam;
        updateUser.UpdateAddress(dto.ToMdl());

        var updateResult = await teamMgr.UpdateMemberAsync(team, updateUser);

        return updateResult.Convert(u => u?.ToDto());
    }


}//Cls

