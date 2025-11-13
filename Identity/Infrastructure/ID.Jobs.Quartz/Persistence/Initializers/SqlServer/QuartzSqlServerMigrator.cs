using DbUp;
using DbUp.Engine;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace ID.Jobs.Quartz.Persistence.Initializers.SqlServer;

internal class QuartzSqlServerMigrator
{
    public static UpgradeEngine Migrate(string connectionString, Dictionary<string, string> variables, ILogger? logger = null)
    {
        Assembly assembly = IdJobsQrzAssemblyReference.Assembly;

        EnsureDatabase.For.SqlDatabase(connectionString);

        const string nsPrefix = "ID.Jobs.Quartz.Persistence.Initializers.SqlServer.Migrations.";

        var scripts = EmbeddedScriptLoader.LoadEmbeddedSqlScripts(assembly, nsPrefix, variables, logger);

        var builder = DeployChanges.To
            .SqlDatabase(connectionString)
            .JournalToSqlTable(QuartzConstants.DbUp.JournalSchema, QuartzConstants.DbUp.JournalTable)
            .WithScripts(scripts);

        builder = logger != null 
            ? builder.LogTo(logger) 
            : builder.LogToConsole();

        return builder.Build();
    }


}//Cls