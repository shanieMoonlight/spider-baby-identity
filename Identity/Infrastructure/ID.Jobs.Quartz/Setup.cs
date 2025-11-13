using ID.Application.Models;
using ID.Jobs.Quartz.AppImps;
using ID.Jobs.Quartz.Persistence;
using ID.Jobs.Quartz.Servers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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

        return services;
    }


}//Cls
