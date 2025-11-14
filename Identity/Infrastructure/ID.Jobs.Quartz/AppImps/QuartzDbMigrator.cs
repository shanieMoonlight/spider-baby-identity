using DbUp.Engine;
using ID.Application.Models;
using ID.Jobs.Quartz.Persistence.Abs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ID.Application.Jobs.Abstractions;
using ID.Jobs.Quartz.Persistence.MigrationNotifications;

namespace ID.Jobs.Quartz.AppImps;

internal class QuartzDbMigrator(
    IOptions<QuartzConfig> _configProvider,
    IDbUpMigrator _dbUpMigrator,
    ILogger<QuartzDbMigrator> _logger,
    IMigrationNotifier _migrationNotifier)
    : IJobsDbMigrator
{
    private readonly QuartzConfig _config = _configProvider.Value;
    private readonly IMigrationNotifier _migrationNotifierLocal = _migrationNotifier;

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

        // Notify subscribers that migrations succeeded. Best-effort: don't let notification failures break the call.
        try
        {
            //Only notify if there were actually scripts applied.
            if (result.Scripts.Any())
                //Let this block so that the caller knows when migrations are done or failed or in an unknown state.
                await _migrationNotifierLocal.NotifySucceededAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Migration succeeded but migration notifier failed. You should restart the app to ensure ID Jobs are running.");
        }
    }

}//Cls






















