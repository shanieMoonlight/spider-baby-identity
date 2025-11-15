using ID.Jobs.Quartz.Persistence.Abs;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ID.Jobs.Quartz.Persistence.Ef;

internal class SqlEfCoreMigrator(
    IDbCommandExecutor _executor,
    IEmbeddedScriptLoader _loader,
    ILogger<SqlEfCoreMigrator> _loggerLocal)
    : IEfCoreMigrator
{

    private const string _journalTable = QuartzConstants.Db.MigrationsJournalTable.Sql.NAME;
    private const string _journalColPrimary = QuartzConstants.Db.MigrationsJournalTable.Sql.Columns.PRIMARY;
    private const string _journalColScriptName = QuartzConstants.Db.MigrationsJournalTable.Sql.Columns.ScriptName;
    private const string _journalColAppliedAt = QuartzConstants.Db.MigrationsJournalTable.Sql.Columns.AppliedAt;
    private const string _schema = QuartzConstants.Db.Schema;

    public async Task<QuartzMigrateResult> MigrateAsync(Dictionary<string, string> variables, CancellationToken cancellationToken = default)
    {
        var applied = new List<string>();
        var skipped = new List<string>();

        try
        {
            // Load scripts from embedded resources (loader performs token replacement)
            var assembly = IdJobsQrzAssemblyReference.Assembly;
            const string nsPrefix = "ID.Jobs.Quartz.Persistence.DbUp.SqlServer.Migrations.";
            var scripts = _loader.LoadEmbeddedSqlScripts(assembly, nsPrefix, variables);

            await _executor.OpenAsync(cancellationToken);

            await EnsureSchemaExistsAsync(cancellationToken);
            await EnsureJournalTableExistsAsync(cancellationToken);

            // Apply scripts in order
            foreach (var s in scripts)
            {
                cancellationToken.ThrowIfCancellationRequested();

                Console.WriteLine($"Processing script {s.Name}... {Environment.NewLine}{s.Contents}");
                Debug.WriteLine($"Processing script {s.Name}... {Environment.NewLine}{s.Contents}");

                // check journal
                var checkCommandText = $@"SELECT TOP(1) 1 FROM [{_schema}].[{_journalTable}] WHERE {_journalColScriptName} = @name;";
                var exists = await _executor.ExecuteScalarAsync(checkCommandText, new Dictionary<string, object?> { ["name"] = s.Name }, cancellationToken);
                if (exists != null)
                {
                    skipped.Add(s.Name);
                    _loggerLocal.LogDebug("Skipping already applied script {Script}", s.Name);
                    continue;
                }


                try
                {
                    // Split script on lines that contain only 'GO' (case-insensitive) and execute each batch separately
                    var batches = SplitBatches(s.Contents);

                    foreach (var batch in batches)
                    {
                        if (string.IsNullOrWhiteSpace(batch))
                            continue;

                        await _executor.ExecuteNonQueryAsync(batch, null, cancellationToken);
                    }

                    // Insert journal entry
                    var insCommandText = $@"INSERT INTO [{_schema}].[{_journalTable}] ({_journalColScriptName}, {_journalColAppliedAt}) VALUES (@name, SYSDATETIMEOFFSET());";
                    await _executor.ExecuteNonQueryAsync(insCommandText, new Dictionary<string, object?> { ["name"] = s.Name }, cancellationToken);

                    applied.Add(s.Name);
                    _loggerLocal.LogInformation("Applied Quartz migration script {Script}", s.Name);
                }
                catch (Exception ex)
                {
                    _loggerLocal.LogError(ex, "Failed to apply Quartz script {Script}", s.Name);
                    return QuartzMigrateResult.Failure($"Failed applying script {s.Name}", ex);
                }
            }

            return QuartzMigrateResult.Success(applied, skipped);
        }
        catch (Exception ex)
        {
            _loggerLocal.LogError(ex, "SQL Server EF migrator failed");
            return QuartzMigrateResult.Failure("SQL Server EF migrator failed", ex);
        }
    }

    internal static string[] SplitBatches(string contents)
    {
        if (string.IsNullOrEmpty(contents))
            return [];

        var batches = Regex.Split(contents, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
        return [.. batches.Select(b => b.Trim()).Where(b => !string.IsNullOrEmpty(b))];
    }

    //----------------------//

    private async Task EnsureSchemaExistsAsync(CancellationToken cancellationToken)
    {
        var cmdCommandText = $@"
IF SCHEMA_ID(N'{_schema}') IS NULL
    EXEC(N'CREATE SCHEMA [{_schema}]');";


        await _executor.ExecuteNonQueryAsync(cmdCommandText, null, cancellationToken);
    }

    //----------------------//

    private async Task EnsureJournalTableExistsAsync(CancellationToken cancellationToken)
    {
        var cmdCommandText = $@"
IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.TABLES
    WHERE TABLE_SCHEMA = N'{_schema}' AND TABLE_NAME = N'{_journalTable}'
)
BEGIN
    CREATE TABLE [{_schema}].[{_journalTable}] (
        {_journalColPrimary} INT PRIMARY KEY IDENTITY(1,1),
        {_journalColScriptName} NVARCHAR(512) NOT NULL,
        {_journalColAppliedAt} DATETIMEOFFSET NOT NULL
    );
END";
        await _executor.ExecuteNonQueryAsync(cmdCommandText, null, cancellationToken);
    }

}//Cls