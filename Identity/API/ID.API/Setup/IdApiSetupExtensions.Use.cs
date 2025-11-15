using ID.Application.Setup;
using ID.Domain.Entities.Teams;
using ID.Infrastructure.Setup;
using Microsoft.AspNetCore.Builder;
using System.Diagnostics;


namespace ID.API.Setup;

/// <summary>
/// Extension methods for setting up MyId services and middleware in an ASP.NET Core application.
/// </summary>
public  static partial class IdApiSetupExtensions
{

    /// <summary>
    /// Configures the specified IApplicationBuilder to use MyId authentication and authorization.
    /// </summary>
    /// <param name="app">The IApplicationBuilder to configure.</param>
    /// <param name="minTypeDashboardAccess">The Job dashboard is protected. This is the minimum level required to access it. No auth required in Dev Mode</param>
    /// <returns>The IApplicationBuilder with MyId authentication and authorization configured.</returns>
    public static IApplicationBuilder UseMyId(this IApplicationBuilder app, TeamType minTypeDashboardAccess = TeamType.super)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        // This goes AFTER regular auth because we need the user to be authenticated or not before this point
        try
        {
            app.UseMyIdApplication();
            app.UseMyIdInfrastructure(minTypeDashboardAccess);

        }
        catch (Exception e)
        {
            // This can be caused if the DB has not been initialized yet
            Debug.WriteLine($"{e.Message} - {e.StackTrace}");
            Console.WriteLine($"{e.Message} - {e.StackTrace}");
        }

        return app;
    }


}//Cls
