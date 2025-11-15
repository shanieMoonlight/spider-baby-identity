using ID.Domain.Entities.Teams;
using ID.Jobs.Quartz;
using Microsoft.AspNetCore.Builder;

namespace ID.Infrastructure.Setup;

public static partial class IdInfrastructureSetupExtensions
{

    /// <summary>
    /// Configures Middleware and Exception Handling for IdInfrastructure
    /// </summary>
    public static IApplicationBuilder UseMyIdInfrastructure(this IApplicationBuilder app, TeamType minTypeDashboardAccess) =>
        app.UseMyIdQuartzJobs(minTypeDashboardAccess);


}//Cls

