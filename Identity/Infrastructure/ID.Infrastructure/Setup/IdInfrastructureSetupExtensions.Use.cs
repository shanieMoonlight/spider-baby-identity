using ID.Domain.Entities.Teams;
using ID.Infrastructure.Jobs;
using Microsoft.AspNetCore.Builder;

namespace ID.Infrastructure.Setup;

public static partial class IdInfrastructureSetupExtensions
{

    /// <summary>
    /// Configures Middleware and Exception Handling for IdInfrastructure
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseMyIdInfrastructure(this IApplicationBuilder app, TeamType minTypeDashboardAccess) =>
        app.UseMyIdJobs(minTypeDashboardAccess);


}//Cls

