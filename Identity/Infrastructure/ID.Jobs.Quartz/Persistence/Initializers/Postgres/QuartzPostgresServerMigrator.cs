using DbUp;
using DbUp.Engine;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace ID.Jobs.Quartz.Persistence.Initializers.Postgres;

internal class QuartzPostgresServerMigrator
{
    public static UpgradeEngine Migrate(string connectionString, Dictionary<string, string> variables, ILogger? logger = null)
    {
        Assembly assembly = IdJobsQrzAssemblyReference.Assembly;
        EnsureDatabase.For.PostgresqlDatabase(connectionString);

        const string nsPrefix = "ID.Jobs.Quartz.Persistence.Initializers.Postgres.Migrations.";

        var scripts = EmbeddedScriptLoader.LoadEmbeddedSqlScripts(assembly, nsPrefix, variables, logger);

        var builder = DeployChanges.To
            .PostgresqlDatabase(connectionString)
            .JournalToSqlTable(QuartzConstants.DbUp.JournalSchema, QuartzConstants.DbUp.JournalTable)
            .WithScripts(scripts);

        builder = logger != null 
            ? builder.LogTo(logger) 
            : builder.LogToConsole();

        return builder.Build();
    }


}//Cls