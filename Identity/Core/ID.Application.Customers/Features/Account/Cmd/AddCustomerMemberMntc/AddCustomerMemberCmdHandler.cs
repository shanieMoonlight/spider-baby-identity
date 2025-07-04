using ClArch.ValueObjects;
using ID.Application.Customers.Features.Common.Dtos.User;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Domain.Utility.Messages;
using MyResults;
using ID.Domain.Entities.AppUsers.ValueObjects;

namespace ID.Application.Customers.Features.Account.Cmd.AddCustomerMemberMntc;

public class AddCustomerMemberCmdHandler_Mntc(IIdentityTeamManager<AppUser> teamMgr)
     : IIdCommandHandler<AddCustomerMemberCmd_Mntc, AppUser_Customer_Dto>
{

    public async Task<GenResult<AppUser_Customer_Dto>> Handle(AddCustomerMemberCmd_Mntc request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var teamId = dto.TeamId;

        var team = await teamMgr.GetByIdWithMembersAsync(teamId, 1000);
        if (team == null)
            return GenResult<AppUser_Customer_Dto>.NotFoundResult(IDMsgs.Error.NotFound<Team>(teamId));

        if (team.TeamType is not TeamType.customer)
            return GenResult<AppUser_Customer_Dto>.Failure(IDMsgs.Error.Teams.NotACustomerTeam(teamId));

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
}

