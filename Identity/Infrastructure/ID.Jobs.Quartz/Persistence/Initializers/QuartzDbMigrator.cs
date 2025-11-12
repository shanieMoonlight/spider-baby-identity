using DbUp.Engine;
using ID.Application.Models;
using ID.Jobs.Quartz.Persistence.Initializers.Postgres;
using ID.Jobs.Quartz.Persistence.Initializers.SqlServer;

namespace ID.Jobs.Quartz.Persistence.Initializers;

internal class QuartzDbMigrator
{

    public static void EnsureSchema(DatabaseType dbType, string connectionString)
    {
        Dictionary<string, string> _variables = new()
        {
            ["schema"] = QuartzConstants.Schema 
        };


        UpgradeEngine upgrader = dbType switch
        {
            DatabaseType.SqlServer => QuartzSqlServerMigrator.Migrate(connectionString, _variables),
            DatabaseType.PostgreSql => QuartzPostgresServerMigrator.Migrate(connectionString, _variables),
            _ => throw new NotSupportedException($"DB type {dbType} not supported for automatic migrations.")
        };


        var result = upgrader.PerformUpgrade();
        if (!result.Successful)
            throw new InvalidOperationException("Quartz DB migrations failed", result.Error);
    }

}//Cls










