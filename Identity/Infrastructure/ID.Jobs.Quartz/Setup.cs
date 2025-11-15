using CrystalQuartz.Application;
using CrystalQuartz.AspNetCore;
using ID.Application.Middleware.ExternalPages;
using ID.Application.Models;
using ID.Domain.Entities.Teams;
using ID.Jobs.Quartz.AppImps;
using ID.Jobs.Quartz.Persistence;
using ID.GlobalSettings.Constants;
using ID.Jobs.Quartz.Retries;
using ID.Jobs.Quartz.Servers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Quartz;

namespace ID.Jobs.Quartz;
public static class Setup
{

    public static IServiceCollection AddMyIdQuartzJobs(
        this IServiceCollection services,
        DatabaseType databaseType,
        string connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        services.AddSingleton(Options.Create(new QuartzConfig(databaseType, connectionString)));


        services.AddQuartzAppImplementations();
        services.AddQuartzPersistence(databaseType);

        services.AddIsolatedQuartz(
            databaseType: databaseType,
            connectionString: connectionString,
            schema: QuartzConstants.Db.Schema,
            tablePrefix: QuartzConstants.Db.TablePrefix
        );

        services.AddRetries();

        return services;
    }

    //- - - - - - - - - - - - - - - //

    /// <summary>
    /// Setup the Channel used to queue log entries
    /// </summary>
    private static IServiceCollection AddRetries(this IServiceCollection services) =>
        services
            .AddSingleton<PendingRetryStore>()
            .AddHostedService<PendingRetriesHostedService>();


    //------------------------------//

    public static IApplicationBuilder UseMyIdQuartzJobs(this IApplicationBuilder app, TeamType minTeamTypeDashboardAccess = TeamType.super)
    {

        switch (minTeamTypeDashboardAccess)
        {
            case TeamType.super:
                app.UseExternalPagesAuth_SuperTeam(IdGlobalConstants.Jobs.DashboardPath);
                break;
            case TeamType.maintenance:
                app.UseExternalPagesAuth_MntcMinimum(IdGlobalConstants.Jobs.DashboardPath);
                break;
            case TeamType.customer:
                app.UseExternalPagesAuth_CustomerMinimum(IdGlobalConstants.Jobs.DashboardPath);
                break;
        }


        app.UseCrystalQuartz(() => app.ApplicationServices.GetRequiredService<ISchedulerFactory>().GetScheduler(QuartzConstants.Scheduler),
            new CrystalQuartzOptions { Path = IdGlobalConstants.Jobs.DashboardPath }
        );

        return app;
    }

}//Cls
