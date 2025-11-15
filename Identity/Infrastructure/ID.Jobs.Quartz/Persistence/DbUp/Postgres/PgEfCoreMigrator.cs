using ID.Jobs.Quartz.Persistence.Abs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;

namespace ID.Jobs.Quartz.Persistence.Ef;

internal class PgEfCoreMigrator(
    IOptions<QuartzConfig> _configProvider,
    IEmbeddedScriptLoader _embeddedScriptLoader,
    ILogger<PgEfCoreMigrator> _logger)
    : IEfCoreMigrator
{
    private readonly QuartzConfig _config = _configProvider.Value;
    private readonly IEmbeddedScriptLoader _loader = _embeddedScriptLoader;
    private readonly ILogger _loggerLocal = _logger;

    private const string _journalTable = QuartzConstants.Db.MigrationsJournalTable.Sql.NAME;
    private const string _journalColPrimary = QuartzConstants.Db.MigrationsJournalTable.Postgres.Columns.PRIMARY;
    private const string _journalColScriptName = QuartzConstants.Db.MigrationsJournalTable.Postgres.Columns.ScriptName;
    private const string _journalColAppliedAt = QuartzConstants.Db.MigrationsJournalTable.Postgres.Columns.AppliedAt;
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
            const string nsPrefix = "ID.Jobs.Quartz.Persistence.DbUp.Postgres.Migrations.";
            var scripts = _loader.LoadEmbeddedSqlScripts(assembly, nsPrefix, variables);

            await using var conn = new NpgsqlConnection(connString);
            await conn.OpenAsync(cancellationToken);

            // Ensure journal table exists

            // Ensure schema exists (script tokens normally do this, but be safe)
            await EnsureSchemaExistsAsync(conn, cancellationToken);
            await EnsureJournalTableExistsAsync(conn, cancellationToken);

            // Apply scripts in order
            foreach (var s in scripts)
            {
                cancellationToken.ThrowIfCancellationRequested();

                // check journal
                await using (var check = conn.CreateCommand())
                {
                    check.CommandText = $@"SELECT 1 FROM ""{_schema}"".""{_journalTable}"" WHERE {_journalColScriptName} = @name LIMIT 1;";
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
                    // Execute script (may contain multiple statements / DO blocks)
                    await using (var exec = conn.CreateCommand())
                    {
                        exec.CommandText = s.Contents;
                        exec.CommandTimeout = 0;
                        await exec.ExecuteNonQueryAsync(cancellationToken);
                    }

                    // Insert journal entry
                    await using (var ins = conn.CreateCommand())
                    {
                        ins.CommandText = $@"INSERT INTO ""{_schema}"".""{_journalTable}"" ({_journalColScriptName}, {_journalColAppliedAt}) VALUES (@name, now());";
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
            _loggerLocal.LogError(ex, "Postgres EF migrator failed");
            return QuartzMigrateResult.Failure("Postgres EF migrator failed", ex);
        }
    }

    //----------------------//

    private static async Task EnsureSchemaExistsAsync(NpgsqlConnection conn, CancellationToken cancellationToken)
    {
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = $@"CREATE SCHEMA IF NOT EXISTS ""{_schema}"";";
        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

    //----------------------//

    private static async Task EnsureJournalTableExistsAsync(NpgsqlConnection conn, CancellationToken cancellationToken)
    {
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = $@"
CREATE TABLE IF NOT EXISTS ""{_schema}"".""{_journalTable}"" (
    {_journalColPrimary} INT GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY,
    {_journalColScriptName} VARCHAR(512),
    {_journalColAppliedAt} TIMESTAMPTZ NOT NULL
);";
        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

}//Cls