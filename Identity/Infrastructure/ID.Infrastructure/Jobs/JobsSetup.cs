using ID.Domain.Entities.Teams;
using ID.Infrastructure.Jobs.Imps.DbMntc;
using ID.Infrastructure.Jobs.Imps.OutboxMsg;
using ID.Infrastructure.Jobs.Service.HangFire;
using ID.Infrastructure.Setup;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ID.Infrastructure.Jobs;

/// <summary>
/// Provides extension methods for configuring background job processing services.
/// </summary>
internal static class JobsSetup
{
    /// <summary>
    /// Sets up background job processing services.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="options">Configuration options for the identity infrastructure.</param>
    /// <returns>The service collection with job services configured.</returns>
    public static IServiceCollection SetupJobs(this IServiceCollection services, IdInfrastructureSetupOptions options)
    {
        return services
            .AddMyIdHangfireJobs(options)
            .AddProcessOutboxMsgJobs()
            .AddDbMntcJobs();
    }


    //----------------------------------//


    /// <summary>
    /// Configures the application to use background job middleware.
    /// </summary>
    /// <param name="app">The application builder to configure.</param>
    /// <param name="minTeamTypeDashboardAccess">The minimum team type required to access the job dashboard.</param>
    /// <returns>The application builder with job middleware configured.</returns>
    public static IApplicationBuilder UseMyIdJobs(this IApplicationBuilder app, TeamType minTeamTypeDashboardAccess)
    {
        return app
            .UseMyIdHangfireJobs(minTeamTypeDashboardAccess);
    }

}
