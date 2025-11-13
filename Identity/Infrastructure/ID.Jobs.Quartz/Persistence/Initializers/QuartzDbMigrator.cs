using DbUp.Engine;
using Microsoft.Extensions.Logging;
using ID.Application.Models;
using ID.Jobs.Quartz.Persistence.Initializers.Postgres;
using ID.Jobs.Quartz.Persistence.Initializers.SqlServer;

namespace ID.Jobs.Quartz.Persistence.Initializers;

internal class QuartzDbMigrator
{

    public static void EnsureSchema(DatabaseType dbType, string connectionString, ILogger? logger = null)
    {
        Dictionary<string, string> _variables = new()
        {
            ["schema"] = QuartzConstants.Schema 
        };


        UpgradeEngine upgrader = dbType switch
        {
            DatabaseType.SqlServer => QuartzSqlServerMigrator.Migrate(connectionString, _variables, logger),
            DatabaseType.PostgreSql => QuartzPostgresServerMigrator.Migrate(connectionString, _variables, logger),
            _ => throw new NotSupportedException($"DB type {dbType} not supported for automatic migrations.")
        };


        var result = upgrader.PerformUpgrade();
        if (!result.Successful)
        {
            logger?.LogError(result.Error, "Quartz DB migrations failed");
            throw new InvalidOperationException("Quartz DB migrations failed", result.Error);
        }

        logger?.LogInformation("Quartz DB migrations completed successfully for {DatabaseType}. See debug logs for prepared script names and DbUp journal for applied scripts.", dbType);
    }

}//Cls










