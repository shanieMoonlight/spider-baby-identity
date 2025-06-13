using ClArch.ValueObjects;
using ID.Application.AppAbs.Setup;
using ID.Application.Utility;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Domain.Utility.Exceptions;
using ID.GlobalSettings.Constants;
using ID.Infrastructure.Persistance.Abstractions.Repos;
using LoggingHelpers;
using Microsoft.Extensions.Logging;
using ID.Domain.AppServices.Abs;
using ID.Domain.Entities.AppUsers.ValueObjects;

namespace ID.Infrastructure.Services.Initialization.UsersAndRoles;

internal class UserAndRoleDataInitializer(
    IIdUnitOfWork _uow, 
    IIdentityTeamManager<AppUser> _teamMgr, 
    ITeamBuilderService _teamBuilder,
    ILogger<UserAndRoleDataInitializer> _logger) 
    : IUserAndRoleDataInitializer
{
    public async Task<string> SeedDataAsync(string superLeaderPassword, string? superLeaderEmail = null)
    {
        if (string.IsNullOrWhiteSpace(superLeaderEmail))
            superLeaderEmail = IdGlobalConstants.Initialization.SUPER_LEADER_EMAIL;
            
        return await SeedUsersAsync(superLeaderPassword, superLeaderEmail);
    }

    //-----------------------//

    private async Task<string> SeedUsersAsync(string superLeaderPassword, string superLeaderEmail)
    {
        try
        {
            var superTeam = await _teamMgr.GetSuperTeamWithMembersAsync();
            superTeam ??= await SeedSuperTeamAsync();

            var superLeader = superTeam.Members
                .FirstOrDefault(m => m.Id == superTeam.LeaderId);
            superLeader ??= await SeedSuperLeaderAsync(superTeam, superLeaderPassword, superLeaderEmail);

            await SeedMntcTeamAsync();

            await _uow.SaveChangesAsync();

            return superLeaderEmail;

        }
        catch (Exception e)
        {
            _logger.LogException(e, MyIdLoggingEvents.STARTUP_ERROR, "STARTUP_ERROR");
            throw;
        }

    }

    //-----------------------//

    public async Task<bool> IsAlreadyInitializedAsync()
    {
        try
        {
            var superTeam = await _teamMgr.GetSuperTeamWithMembersAsync();

            var superLeader = superTeam?.Members?
                .FirstOrDefault(m => m.Id == superTeam.LeaderId);

            return superLeader is not null;
        }
        catch (Exception)
        {
            return false;
        }
    }

    //-----------------------//

    private async Task<AppUser> SeedSuperLeaderAsync(Team superTeam, string superLeaderPassword, string superLeaderEmail)
    {

        var newUser = AppUser.Create(
          superTeam,
          EmailAddress.Create(superLeaderEmail),
          UsernameNullable.Create("superLeader"),
          PhoneNullable.Create("066 666 666 66"),
          FirstNameNullable.Create("Clarke"),
          LastNameNullable.Create("Kent"),
          TeamPositionNullable.Create());

        var createSuperResult = await _teamMgr.RegisterMemberWithPasswordAsync(superTeam, newUser, superLeaderPassword);

        if (!createSuperResult.Succeeded)
            throw new SeedingException(createSuperResult.Info);

        AppUser superUser = createSuperResult.Value!;

        superUser.EmailConfirmed = true;


        return superUser;
    }

    //-----------------------//

    private async Task<Team> SeedSuperTeamAsync()
    {
        Team? superTeam = await _teamMgr.GetSuperTeamWithMembersAsync();
        if (superTeam != null)
            return superTeam;

        superTeam = _teamBuilder.CreateSuperTeam();

        var dbSuperTeam = await _teamMgr.AddTeamAsync(superTeam);

        return dbSuperTeam;
    }

    //-----------------------//

    private async Task<Team> SeedMntcTeamAsync()
    {
        Team? adminTeam = await _teamMgr.GetMntcTeamWithMembersAsync(10000);
        if (adminTeam != null)
            return adminTeam;


        adminTeam = _teamBuilder.CreateMaintenanceTeam();

        await _teamMgr.AddTeamAsync(adminTeam);

        return adminTeam;

    }

    //-----------------------//

}//Cls