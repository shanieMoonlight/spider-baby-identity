using ID.Application.Models;
using ID.Jobs.Quartz.Imps;
using ID.Jobs.Quartz.Persistence.Initializers;
using ID.Jobs.Quartz.Servers;
using Microsoft.Extensions.DependencyInjection;

namespace ID.Jobs.Quartz;
public static class Setup
{

    public static IServiceCollection AddMyIdQuartzJobs(
        this IServiceCollection services,
        DatabaseType databaseType,
        string connectionString,
        bool ensureDb = true)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        // Optionally ensure DB objects exist (opt-in)
        if (ensureDb)
            QuartzDbMigrator.EnsureSchema(databaseType, connectionString);

        services.AddMyIdQuartzJobs();

        services.AddIsolatedQuartz(
            databaseType: databaseType,
            connectionString: connectionString, 
            schema: QuartzConstants.Schema,
            tablePrefix: QuartzConstants.TablePrefix
        );

        return services;
    }



}//Cls
