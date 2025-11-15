using ID.Jobs.Quartz.Persistence.Abs;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ID.Jobs.Quartz.Persistence.Ef;

internal class SqlEfCoreMigrator(
    IOptions<QuartzConfig> _configProvider,
    IEmbeddedScriptLoader _loader,
    ILogger<SqlEfCoreMigrator> _loggerLocal)
    : IEfCoreMigrator
{
    private readonly QuartzConfig _config = _configProvider.Value;

    private const string _journalTable = QuartzConstants.Db.MigrationsJournalTable.Sql.NAME;
    private const string _journalColPrimary = QuartzConstants.Db.MigrationsJournalTable.Sql.Columns.PRIMARY;
    private const string _journalColScriptName = QuartzConstants.Db.MigrationsJournalTable.Sql.Columns.ScriptName;
    private const string _journalColAppliedAt = QuartzConstants.Db.MigrationsJournalTable.Sql.Columns.AppliedAt;
    private const string _schema = QuartzConstants.Db.Schema;

    public async Task<QuartzMigrateResult> MigrateAsync(Dictionary<string, string> variables, CancellationToken cancellationToken = default)
    {
        var connString = _config.ConnectionString;
        var applied = new List<string>();
        var skipped = new List<string>();

        try
        {
            // Load scripts from embedded resources (loader performs token replacement)
            var assembly = IdJobsQrzAssemblyReference.Assembly;
            const string nsPrefix = "ID.Jobs.Quartz.Persistence.DbUp.SqlServer.Migrations.";
            var scripts = _loader.LoadEmbeddedSqlScripts(assembly, nsPrefix, variables);

            await using var conn = new SqlConnection(connString);
            await conn.OpenAsync(cancellationToken);

            await EnsureSchemaExistsAsync(conn, cancellationToken);
            await EnsureJournalTableExistsAsync(conn, cancellationToken);

            // Apply scripts in order
            foreach (var s in scripts)
            {
                cancellationToken.ThrowIfCancellationRequested();

                Console.WriteLine($"Processing script {s.Name}... {Environment.NewLine}{s.Contents}");
                Debug.WriteLine($"Processing script {s.Name}... {Environment.NewLine}{s.Contents}");

                // check journal
                await using (var check = conn.CreateCommand())
                {
                    check.CommandText = $@"SELECT TOP(1) 1 FROM [{_schema}].[{_journalTable}] WHERE {_journalColScriptName} = @name;";
                    var p = check.CreateParameter();
                    p.ParameterName = "name";
                    p.Value = s.Name;
                    check.Parameters.Add(p);

                    var exists = await check.ExecuteScalarAsync(cancellationToken);
                    if (exists != null)
                    {
                        skipped.Add(s.Name);
                        _loggerLocal.LogDebug("Skipping already applied script {Script}", s.Name);
                        continue;
                    }
                }

                try
                {
                    // Split script on lines that contain only 'GO' (case-insensitive) and execute each batch separately
                    var batches = Regex.Split(s.Contents, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

                    foreach (var batch in batches)
                    {
                        if (string.IsNullOrWhiteSpace(batch))
                            continue;

                        await using var exec = conn.CreateCommand();
                        exec.CommandText = batch;
                        exec.CommandTimeout = 0;
                        await exec.ExecuteNonQueryAsync(cancellationToken);
                    }

                    // Insert journal entry
                    await using (var ins = conn.CreateCommand())
                    {
                        ins.CommandText = $@"INSERT INTO [{_schema}].[{_journalTable}] ({_journalColScriptName}, {_journalColAppliedAt}) VALUES (@name, SYSDATETIMEOFFSET());";
                        var p = ins.CreateParameter();
                        p.ParameterName = "name";
                        p.Value = s.Name;
                        ins.Parameters.Add(p);
                        await ins.ExecuteNonQueryAsync(cancellationToken);
                    }

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

    //----------------------//

    private static async Task EnsureSchemaExistsAsync(SqlConnection conn, CancellationToken cancellationToken)
    {
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = $@"
IF SCHEMA_ID(N'{_schema}') IS NULL
    EXEC(N'CREATE SCHEMA [{_schema}]');";
        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

    //----------------------//

    private static async Task EnsureJournalTableExistsAsync(SqlConnection conn, CancellationToken cancellationToken)
    {
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = $@"
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
        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

}//Cls