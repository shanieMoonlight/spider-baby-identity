using DbUp.Engine;
using ID.Application.Models;
using ID.Jobs.Quartz.Persistence.Abs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ID.Application.Jobs.Abstractions;

namespace ID.Jobs.Quartz.AppImps;

internal class QuartzDbMigrator(IOptions<QuartzConfig> _configProvider, IDbUpMigrator _dbUpMigrator, ILogger<QuartzDbMigrator> _logger) 
    : IJobsDbMigrator
{

    private readonly QuartzConfig _config = _configProvider.Value;

    //----------------------//

    public async Task MigrateAsync(CancellationToken cancellationToken)
    {
        DatabaseType dbType = _config.DatabaseType;
        string connectionString = _config.ConnectionString;
        Dictionary<string, string> _variables = new()
        {
            ["schema"] = QuartzConstants.Schema
        };


        UpgradeEngine upgrader = await _dbUpMigrator.MigrateAsync(_variables, cancellationToken);

        var result = upgrader.PerformUpgrade();
        if (!result.Successful)
        {
            _logger.LogError(result.Error, "Quartz DB migrations failed for {DatabaseType}. See debug logs for prepared script names and DbUp journal for applied scripts. ConnectionString: {ConnectionString}", dbType, connectionString);
            throw new InvalidOperationException("Quartz DB migrations failed", result.Error);
        }

        _logger.LogInformation("Quartz DB migrations completed successfully for {DatabaseType}. See debug logs for prepared script names and DbUp journal for applied scripts. ConnectionString: {ConnectionString}", dbType, connectionString);

    }

}//Cls






















