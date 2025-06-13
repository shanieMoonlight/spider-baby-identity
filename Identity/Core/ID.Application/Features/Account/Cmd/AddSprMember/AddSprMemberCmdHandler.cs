using ClArch.ValueObjects;
using ID.Application.Features.Common.Dtos.User;
using ID.Application.Dtos.User;
using MyResults;
using ID.Application.AppAbs.Permissions;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Entities.AppUsers.ValueObjects;

namespace ID.Application.Features.Account.Cmd.AddSprMember;
public class AddSprMemberCmdHandler(IIdentityTeamManager<AppUser> teamMgr, IAppPermissionService<AppUser> _appPermissions)
    : IIdCommandHandler<AddSprMemberCmd, AppUserDto>
{

    public async Task<GenResult<AppUserDto>> Handle(AddSprMemberCmd request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var canAddResult = await _appPermissions.AddPermissions
            .CanAddTeamMember(dto.TeamPosition, request);

        if (!canAddResult.Succeeded)
            return canAddResult.Convert<AppUserDto>();

        var sprTeam = request.PrincipalTeam!;

        var newUser = AppUser.Create(
            sprTeam,
            EmailAddress.Create(dto.Email),
            UsernameNullable.Create(dto.Username),
            PhoneNullable.Create(dto.PhoneNumber),
            FirstNameNullable.Create(dto.FirstName),
            LastNameNullable.Create(dto.LastName),
            TeamPositionNullable.Create(dto.TeamPosition));


        var createResult = await teamMgr.RegisterMemberAsync(sprTeam, newUser);
        return createResult.Convert(user => user?.ToDto());

    }

}//Cls

