using Hangfire;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using MyIdDemo.Setup.Data;
using MyIdDemo.Setup.Utils;

namespace MyIdDemo.Setup;



//######################################//


/// <summary>
/// Install Hangfire stuff
/// </summary>
public class DemoHangfireInstaller : IServiceInstaller
{
    public WebApplicationBuilder Install(WebApplicationBuilder builder, StartupData startupData)
    {
        var services = builder.Services;
        services.AddHangfire(x => x
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(
                startupData.ConnectionStringsSection.GetSqlDb(),
                new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true,
                }));

        // Add the processing server as IHostedService
        services.AddHangfireServer();
        return builder;
    }
}

//######################################//

internal static class HangfireJobsInstaller
{
    /// <summary>
    /// Configures Middleware and Exception Handling for IdInfrastructure
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <param name="startupData">All the app config and settings.</param>
    /// <param name="customAuthFilters">Custom authorization filters for the Hangfire dashboard.</param>
    /// <returns>The application builder.</returns>
    public static IApplicationBuilder UseDemoHangfireJobs(
        this IApplicationBuilder app,
        StartupData startupData,
        IEnumerable<IDashboardAuthorizationFilter>? customAuthFilters = null)
    {
        var dashboardOptions = new DashboardOptions
        {
            DashboardTitle = startupData.HangFireSection.GetDashboardTitle(),
            AppPath = startupData.HangFireSection.GetBackToSitePath(),
            StatsPollingInterval = 60000, // 1 minute
        };

        if (customAuthFilters != null && customAuthFilters.Any())
            dashboardOptions.Authorization = [.. customAuthFilters];

        var dashboardPath = startupData.HangFireSection.GetDashboardPath();

        app.UseHangfireDashboard(dashboardPath, dashboardOptions);

        return app;
    }


}//Cls


//######################################//


/// <summary>
/// Install Identity stuff
/// </summary>
public static class DemoHangfireInstallerExtensions
{

    /// <summary>
    /// Install some Identity dependencies
    /// </summary>
    /// <param name="builder">Application Builder</param>
    /// <param name="startupData">All the app config and settings</param>
    public static WebApplicationBuilder InstallHangfire(this WebApplicationBuilder builder, StartupData startupData) =>
        new DemoHangfireInstaller().Install(builder, startupData);

}//Cls



//######################################//
