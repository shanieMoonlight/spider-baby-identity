using ClArch.ValueObjects;
using ID.Application.Features.Common.Dtos.User;
using ID.Application.Dtos.User;
using MyResults;
using ID.Application.AppAbs.Permissions;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Utility.Messages;
using ID.Domain.Entities.AppUsers.ValueObjects;
using ID.GlobalSettings.Constants;

namespace ID.Application.Features.Account.Cmd.AddMntcMember;
public class AddMntcMemberCmdHandler(IIdentityTeamManager<AppUser> _teamMgr, IAppPermissionService<AppUser> _appPermissions)
    : IIdCommandHandler<AddMntcMemberCmd, AppUserDto>
{

    public async Task<GenResult<AppUserDto>> Handle(AddMntcMemberCmd request, CancellationToken cancellationToken)
    {

        var dto = request.Dto;
        var teamPosition = request.IsSuperMinimum 
            ? IdGlobalConstants.Teams.CATCH_ALL_MAX_POSITION 
            : request.PrincipalTeamPosition;

        //Super can add to Mntc
        if (!request.IsSuper)
        {
            //If Principal is not in Super Team it must pass CanAddTeamMember
            var canAddResult = await _appPermissions.AddPermissions
                .CanAddTeamMember(dto.TeamPosition, request);

            if (!canAddResult.Succeeded)
                return canAddResult.Convert<AppUserDto>();
        }


        var mntcTeam = await _teamMgr.GetMntcTeamWithMembersAsync(teamPosition);
        if (mntcTeam is null)
            return GenResult<AppUserDto>.NotFoundResult(IDMsgs.Error.NOT_FOUND_MNTC_TEAM);


        var newUser = AppUser.Create(
            mntcTeam,
            EmailAddress.Create(dto.Email),
            UsernameNullable.Create(dto.Username),
            PhoneNullable.Create(dto.PhoneNumber),
            FirstNameNullable.Create(dto.FirstName),
            LastNameNullable.Create(dto.LastName),
            TeamPositionNullable.Create(dto.TeamPosition));

        //mntcTeam.AddMember(newUser);


        var createResult = await _teamMgr.RegisterMemberAsync(mntcTeam, newUser);
        return createResult.Convert(user => user?.ToDto());
    }


}//Cls

