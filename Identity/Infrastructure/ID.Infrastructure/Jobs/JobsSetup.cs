using ID.Application.Jobs.Abstractions;
using ID.Application.Models;
using ID.Domain.Entities.Teams;
using ID.Infrastructure.Jobs.Imps;
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
    public static IServiceCollection SetupJobs(this IServiceCollection services, DatabaseType databaseType, IdInfrastructureSetupOptions options)
    {
        services.AddScoped<ICronBuilder, HfCronBuilder>();
        services.AddMyIdHangfireJobs(databaseType, options);
        return services;
    }


    //-------------------------//


    /// <summary>
    /// Configures the application to use background job middleware.
    /// </summary>
    /// <param name="app">The application builder to configure.</param>
    /// <param name="minTeamTypeDashboardAccess">The minimum team type required to access the job dashboard.</param>
    /// <returns>The application builder with job middleware configured.</returns>
    public static IApplicationBuilder UseMyIdJobs(this IApplicationBuilder app, TeamType minTeamTypeDashboardAccess) =>
        app.UseMyIdHangfireJobs(minTeamTypeDashboardAccess);

}//Cls
