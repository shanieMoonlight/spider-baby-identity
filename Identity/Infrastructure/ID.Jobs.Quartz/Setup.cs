using CrystalQuartz.Application;
using CrystalQuartz.AspNetCore;
using ID.Application.Middleware.ExternalPages;
using ID.Application.Models;
using ID.Domain.Entities.Teams;
using ID.Jobs.Quartz.AppImps;
using ID.Jobs.Quartz.Persistence;
using ID.Jobs.Quartz.Servers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using System.Diagnostics;
using System.Threading.Channels;
using ID.Jobs.Quartz.AppImps.Migration;

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
            schema: QuartzConstants.Schema,
            tablePrefix: QuartzConstants.TablePrefix
        );

        // register migration notifier for other components to subscribe
        services.AddSingleton<IMigrationNotifier, InMemoryMigrationNotifier>();

        services.RegisterChannel();

        return services;
    }

    /// <summary>
    /// Setup the Channel used to queue log entries
    /// </summary>
    private static IServiceCollection RegisterChannel(this IServiceCollection services)
    {
        // register channel singleton
        services.AddSingleton(Channel.CreateBounded<PendingRetry>(new BoundedChannelOptions(32)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,
            SingleWriter = false
        }));

        // convenience registration: inject ChannelWriter<PendingRetry> where you enqueue
        services.AddSingleton(provider => provider.GetRequiredService<Channel<PendingRetry>>().Writer);

        // register the consumer hosted service that will process retries
        services.AddHostedService<PendingRetriesHostedService>();

        return services;
    }


    //----------------------------------//

    public static IApplicationBuilder UseMyIdQuartzJobs(this IApplicationBuilder app, TeamType minTeamTypeDashboardAccess = TeamType.super)
    {

        switch (minTeamTypeDashboardAccess)
        {
            case TeamType.super:
                app.UseExternalPagesAuth_SuperTeam("/myid-jobs-dashboard");
                break;
            case TeamType.maintenance:
                app.UseExternalPagesAuth_MntcMinimum("/myid-jobs-dashboard");
                break;
            case TeamType.customer:
                app.UseExternalPagesAuth_CustomerMinimum("/myid-jobs-dashboard");
                break;
        }

        app.UseCrystalQuartz(() => app.ApplicationServices.GetRequiredService<ISchedulerFactory>().GetScheduler(),
            new CrystalQuartzOptions { Path = "/myid-jobs-dashboard" }
        );

        return app;
    }

}//Cls
