using ID.Application.Models;
using ID.Jobs.Quartz.Imps;
using ID.Jobs.Quartz.Persistence.Initializers;
using ID.Jobs.Quartz.Servers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
        {
            // Build a temporary provider to get an ILogger instance for migration logging.
            using var sp = services.BuildServiceProvider();
            var logger = sp.GetService<ILogger<QuartzDbMigrator>>();
            QuartzDbMigrator.EnsureSchema(databaseType, connectionString, logger);
        }

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
