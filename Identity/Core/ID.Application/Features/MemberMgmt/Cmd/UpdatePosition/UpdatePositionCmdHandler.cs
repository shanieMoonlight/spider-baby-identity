using ID.Application.Features.Common.Dtos.User;
using ID.Application.Dtos.User;
using ID.Application.Features.Teams;
using MyResults;
using ID.Application.AppAbs.Permissions;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Entities.AppUsers.ValueObjects;
using ID.Domain.Entities.Teams.Validators;

namespace ID.Application.Features.MemberMgmt.Cmd.UpdatePosition;
public class UpdatePositionCmdHandler(
    IIdentityTeamManager<AppUser> _teamMgr,
    IAppPermissionService<AppUser> _appPermissions)
    : IIdCommandHandler<UpdatePositionCmd, AppUserDto>
{

    public async Task<GenResult<AppUserDto>> Handle(UpdatePositionCmd request, CancellationToken cancellationToken)
    {
        var newPositionUserId = request.Dto.UserId;
        var newPosition = request.Dto.NewPosition;
        var teamId = request.PrincipalTeamId;
        var team = request.PrincipalTeam;


        var canCanChangePositionResult = await _appPermissions.ChangePositionPermissions
            .CanChangePositionAsync(newPositionUserId, newPosition, request);

        if (!canCanChangePositionResult.Succeeded)
            return canCanChangePositionResult.Convert<AppUserDto>();

        var newPositionUser = canCanChangePositionResult.Value!; //Success is non-null

        // Validate the position range update
        var validationResult = TeamValidators.MemberPositionUpdate.Validate(team, newPositionUser, TeamPosition.Create(newPosition));
        if (!validationResult.Succeeded)
            return GenResult<AppUserDto>.BadRequestResult(validationResult.Info);

        // Apply the validated change
        team.UpdateMemberPosition(validationResult.Value!);
        var chPosResult = await _teamMgr.UpdateMemberAsync(team, newPositionUser);

        return chPosResult.Convert(u => u?.ToDto());
    }

}//Cls

