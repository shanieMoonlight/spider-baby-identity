using ID.TeamRoles.UserToAdmin.Authenticators;
using Microsoft.Extensions.DependencyInjection;

namespace ID.TeamRoles.UserToAdmin.Setup;

public static class TeamRolesUserToAdminSetupExtensions
{
    /// <summary>
    ///  Setup TeamRolesUserToAdmin
    /// </summary>
    /// <returns>The same services</returns>
    public static IServiceCollection AddTeamRolesUserToAdmin(this IServiceCollection services, TeamRolesUserToAdminSetupOptions? setupOptions = null)
    {
        return services
            .AddTeamRolePolicies();
    }

    //- - - - - - - - - - - - - - - - - - //

    /// <summary>
    ///  Setup TeamRolesUserToAdmin
    /// </summary>
    /// <returns>The same services</returns>
    public static IServiceCollection AddTeamRolesUserToAdmin(this IServiceCollection services, Action<TeamRolesUserToAdminSetupOptions> config)
    {
        TeamRolesUserToAdminSetupOptions setupOptions = new();
        config(setupOptions);

        return services.AddTeamRolesUserToAdmin(setupOptions);
    }



}//Cls

