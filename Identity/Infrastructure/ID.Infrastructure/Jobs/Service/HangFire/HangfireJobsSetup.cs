using Hangfire;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using ID.Application.Jobs.Abstractions;
using ID.Domain.Entities.Teams;
using ID.Infrastructure.Jobs.Service.HangFire.Auth;
using ID.Infrastructure.Jobs.Service.HangFire.Background;
using ID.Infrastructure.Jobs.Service.HangFire.Instance.Abs;
using ID.Infrastructure.Jobs.Service.HangFire.Instance.Imps;
using ID.Infrastructure.Jobs.Service.HangFire.Recurring;
using ID.Infrastructure.Setup;
using ID.Infrastructure.Utility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace ID.Infrastructure.Jobs.Service.HangFire;

internal static class HangfireJobsSetup
{
    internal static IServiceCollection AddMyIdHangfireJobs(this IServiceCollection services, IdInfrastructureSetupOptions options)
    {
        var storageOptions = new SqlServerStorageOptions
        {
            CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
            QueuePollInterval = TimeSpan.Zero,
            UseRecommendedIsolationLevel = true,
            DisableGlobalLocks = true,
            SchemaName = IdInfrastructureConstants.Jobs.Schema // MyId's dedicated schema
        };

        // Create the SqlServerStorage instance for MyId's jobs
        // Use JobStorage instead of var for proper DI
        JobStorage myIdHfStorage = new SqlServerStorage(options.ConnectionString, storageOptions);

        // Register MyId's specific storage with a KEY.
        // This makes it available for specific resolution via [FromKeyedServices].
        services.AddKeyedSingleton(IdInfrastructureConstants.Jobs.DI_StorageKey, myIdHfStorage);

        // Configure MyId's server explicitly using MyId's storage
        // This server will only process jobs from MyId's schema and queues.
        services.AddHangfireServer((provider, config) =>
        {
            config.ServerName = IdInfrastructureConstants.Jobs.Server;
            config.Queues = IdInfrastructureConstants.Jobs.Queues.All; // Queues specific to MyId
        }, myIdHfStorage); // Explicitly use myIdHfStorage for this server


        // Register the storage provider so that it can be injected into job managers
        services.AddScoped<IHangfireInstanceProvider, HangfireInstanceProvider>();

        services.AddMyIdHangfireRecurringJobs();
        services.AddMyIdHangfireBackgroundJobs();

        services.AddScoped<IMyIdJobService, HangFireJobService>(); 
        return services;
    }


    //- - - - - - - - - - - - - - - - - -//


    /// <summary>
    /// Configures Middleware and Exception Handling for IdInfrastructure
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    internal static IApplicationBuilder UseMyIdHangfireJobs(this IApplicationBuilder app, TeamType minType)
    {
        IDashboardAuthorizationFilter? customAuthFilter = minType switch
        {
            TeamType.Customer => new HangFire_Authenticated_OrDev_AuthorizationFilter(),
            TeamType.Maintenance => new HangFire_MntcMin_OrDev_AuthorizationFilter(),
            TeamType.Super => new HangFire_SuperMin_OrDev_AuthorizationFilter(),
            _ => new HangFire_Authenticated_OrDev_AuthorizationFilter(),
        };

        // Get our specific storage instance
        var myIdHfStorage = app.ApplicationServices.GetKeyedService<JobStorage>(IdInfrastructureConstants.Jobs.DI_StorageKey);


        var dashboardOptions = new DashboardOptions
        {
            DashboardTitle = "MyId Jobs Dashboard",
            AppPath = IdInfrastructureConstants.Jobs.BackToAppPath,
            Authorization = [customAuthFilter],
            StatsPollingInterval = 60000, // 1 minute
        };


        if (app is IEndpointRouteBuilder endpointRouteBuilder)
            endpointRouteBuilder.MapHangfireDashboard(
                    IdInfrastructureConstants.Jobs.DashboardPath,
                    dashboardOptions,
                    myIdHfStorage);
        else
            app.UseHangfireDashboard(
                IdInfrastructureConstants.Jobs.DashboardPath,
                dashboardOptions,
                myIdHfStorage);


        return app;

    }


}//Cls
