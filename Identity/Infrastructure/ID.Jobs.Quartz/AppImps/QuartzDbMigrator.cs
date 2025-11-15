using ID.Application.Models;
using ID.Jobs.Quartz.Persistence.Abs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ID.Application.Jobs.Abstractions;
using ID.Jobs.Quartz.Persistence.MigrationNotifications;

namespace ID.Jobs.Quartz.AppImps;

internal class QuartzDbMigrator(
    IOptions<QuartzConfig> _configProvider,
    IEfCoreMigrator _efCoreMigrator,
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
            ["schema"] = QuartzConstants.Db.Schema
        };

        var result = await _efCoreMigrator.MigrateAsync(_variables, cancellationToken);

        if (!result.Succeeded)
        {
            _logger.LogError(result.Exception, "Quartz DB migrations failed for {DatabaseType}. Details: {ErrorMessage}. ConnectionString: {ConnectionString}", dbType, result.ErrorMessage, connectionString);
            throw new InvalidOperationException("Quartz DB migrations failed", result.Exception ?? new Exception(result.ErrorMessage ?? "Unknown migration error"));
        }

        // Notify subscribers that migrations succeeded. Best-effort: don't let notification failures break the call.
        try
        {
            //Only notify if there were actually scripts applied.
            if (result.AppliedScripts != null && result.AppliedScripts.Any())
                //Let this block so that the caller knows when migrations are done or failed or in an unknown state.
                await _migrationNotifierLocal.NotifySucceededAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Migration succeeded but migration notifier failed. You should restart the app to ensure ID Jobs are running.");
        }
    }

}//Cls






















