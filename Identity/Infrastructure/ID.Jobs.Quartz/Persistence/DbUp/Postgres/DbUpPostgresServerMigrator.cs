using DbUp;
using DbUp.Engine;
using ID.Jobs.Quartz.AppImps;
using ID.Jobs.Quartz.Persistence.Abs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace ID.Jobs.Quartz.Persistence.DbUp.Postgres;

internal class DbUpPostgresServerMigrator(IOptions<QuartzConfig> _configProvider, ILogger<QuartzDbMigrator> _logger) : IDbUpMigrator
{
    private readonly QuartzConfig _config = _configProvider.Value;

    //----------------------//

    public Task<UpgradeEngine> MigrateAsync(Dictionary<string, string> variables, CancellationToken cancellationToken)
    {
        string connectionString = _config.ConnectionString;

        Assembly assembly = IdJobsQrzAssemblyReference.Assembly;
        EnsureDatabase.For.PostgresqlDatabase(connectionString);

        const string nsPrefix = "ID.Jobs.Quartz.Persistence.DbUp.Postgres.Migrations.";

        var scripts = EmbeddedScriptLoader.LoadEmbeddedSqlScripts(assembly, nsPrefix, variables, _logger);

        var builder = DeployChanges.To
            .PostgresqlDatabase(connectionString)
            .JournalToSqlTable(QuartzConstants.DbUp.JournalSchema, QuartzConstants.DbUp.JournalTable)
            .WithScripts(scripts);

        builder = _logger != null 
            ? builder.LogTo(_logger) 
            : builder.LogToConsole();

        var upgrader = builder.Build();
        return Task.FromResult(upgrader);
    }


}//Cls