using ClArch.ValueObjects;
using ID.Application.AppAbs.Permissions;
using ID.Application.Customers.Features.Common.Dtos.User;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.AppUsers.ValueObjects;
using MyResults;

namespace ID.Application.Customers.Features.Account.Cmd.AddCustomerMember;
public class AddCustomerMemberCmdHandler(IIdentityTeamManager<AppUser> teamMgr, IAppPermissionService<AppUser> _appPermissions)
    : IIdCommandHandler<AddCustomerMemberCmd, AppUser_Customer_Dto>
{

    public async Task<GenResult<AppUser_Customer_Dto>> Handle(AddCustomerMemberCmd request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var canAddResult = await _appPermissions.AddPermissions
            .CanAddCustomerTeamMember(dto.TeamPosition, request);

        if (!canAddResult.Succeeded)
            return canAddResult.Convert<AppUser_Customer_Dto>();

        var team = request.PrincipalTeam!;

        var newUser = AppUser.Create(
          team,
          EmailAddress.Create(dto.Email),
          UsernameNullable.Create(dto.Username),
          PhoneNullable.Create(dto.PhoneNumber),
          FirstNameNullable.Create(dto.FirstName),
          LastNameNullable.Create(dto.LastName),
          TeamPositionNullable.Create(dto.TeamPosition));


        var createResult = await teamMgr.RegisterMemberAsync(team, newUser);
        return createResult.Convert(user => user?.ToCustomerDto());
    }


}//Cls

