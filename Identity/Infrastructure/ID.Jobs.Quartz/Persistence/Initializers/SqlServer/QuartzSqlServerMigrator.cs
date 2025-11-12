using DbUp;
using DbUp.Engine;
using System.Diagnostics;
using System.Reflection;

namespace ID.Jobs.Quartz.Persistence.Initializers.SqlServer;

internal class QuartzSqlServerMigrator
{
    public static UpgradeEngine Migrate(string connectionString, Dictionary<string, string> variables)
    {
        Assembly assembly = IdJobsQrzAssemblyReference.Assembly;
        
        EnsureDatabase.For.SqlDatabase(connectionString);

        const string nsPrefix = "ID.Jobs.Quartz.Persistence.Initializers.SqlServer.Migrations.";


        return DeployChanges.To
            .SqlDatabase(connectionString)
            .JournalToSqlTable(QuartzConstants.DbUp.JournalSchema, QuartzConstants.DbUp.JournalTable)
            .WithScriptsEmbeddedInAssembly(assembly, name => name.StartsWith(nsPrefix, StringComparison.OrdinalIgnoreCase))
            .WithVariables(variables)
            .LogToConsole()
            .Build();
    }


}//Cls