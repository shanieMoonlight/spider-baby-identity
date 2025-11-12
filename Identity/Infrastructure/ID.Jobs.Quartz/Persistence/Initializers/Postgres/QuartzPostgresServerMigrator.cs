using DbUp;
using DbUp.Engine;
using System.Reflection;

namespace ID.Jobs.Quartz.Persistence.Initializers.Postgres;

internal class QuartzPostgresServerMigrator
{
    public static UpgradeEngine Migrate(string connectionString, Dictionary<string, string> variables)
    {
        Assembly assembly = IdJobsQrzAssemblyReference.Assembly;
        EnsureDatabase.For.PostgresqlDatabase(connectionString);
        return DeployChanges.To
            .PostgresqlDatabase(connectionString)
            .JournalToSqlTable(QuartzConstants.DbUp.JournalSchema, QuartzConstants.DbUp.JournalTable)
            .WithScriptsEmbeddedInAssembly(assembly, name => name.StartsWith("ID.Jobs.Quartz.Persistence.Initializers.Postgres.Migrations.", StringComparison.OrdinalIgnoreCase))
            .WithVariables(variables)
            .LogToConsole()
            .Build();
    }


}//Cls