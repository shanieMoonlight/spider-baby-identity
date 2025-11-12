using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;

namespace ID.Jobs.Quartz.Persistence.Initializers.SQL;

internal static class QuartzDbInitializer_SQL
{
    private const string _canonicalTable = "QRTZ_JOB_DETAILS";

    // Public default resource name for the embedded SQL script in this assembly
    public const string DefaultEmbeddedResourceName = "ID.Jobs.Quartz.Persistence.Initializers.SQL.QUARTZ_INIT_SQL.sql";

    //-----------------------//

    public static async Task EnsureQuartzSchemaAsyncFromEmbeddedResource(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString)) 
            throw new ArgumentNullException(nameof(connectionString));

        var assembly = IdJobsQrzAssemblyReference.Assembly;
        await using var stream = assembly.GetManifestResourceStream(DefaultEmbeddedResourceName)
            ?? throw new FileNotFoundException($"Embedded resource '{DefaultEmbeddedResourceName}' not found in {assembly.FullName}.");

        string sql;
        using (var rdr = new StreamReader(stream))
            sql = await rdr.ReadToEndAsync().ConfigureAwait(false);

        await EnsureQuartzSchemaAsyncFromString(connectionString, sql).ConfigureAwait(false);
    }

    //-----------------------//

    public static async Task EnsureQuartzSchemaAsyncFromString(string connectionString, string sql)
    {
        if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentNullException(nameof(connectionString));
        if (string.IsNullOrWhiteSpace(sql)) throw new ArgumentNullException(nameof(sql));

        string schema = QuartzConstants.SCHEMA;

        await using var conn = new SqlConnection(connectionString);
        await conn.OpenAsync().ConfigureAwait(false);

        if (await CanonicalTableExists(conn, schema).ConfigureAwait(false))
            return;

        await CreateSchemaIfMissing(conn, schema).ConfigureAwait(false);
        await ExecuteSqlBatches(conn, sql).ConfigureAwait(false);
    }

    //-----------------------//

    private static async Task<bool> CanonicalTableExists(SqlConnection conn, string schema)
    {
        var checkSql = @"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = @schema AND TABLE_NAME = @table;";
        await using var cmd = new SqlCommand(checkSql, conn);
        cmd.Parameters.AddWithValue("@schema", schema);
        cmd.Parameters.AddWithValue("@table", _canonicalTable);
        var exists = Convert.ToInt32(await cmd.ExecuteScalarAsync().ConfigureAwait(false)) > 0;
        return exists;
    }

    //-----------------------//

    private static async Task CreateSchemaIfMissing(SqlConnection conn, string schema)
    {
        var createSchemaSql = @"
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = @schema)
BEGIN
    DECLARE @quoted nvarchar(300) = QUOTENAME(@schema);
    DECLARE @ddl nvarchar(max) = N'CREATE SCHEMA ' + @quoted;
    EXEC(@ddl);
END
";
        await using var cmd = new SqlCommand(createSchemaSql, conn);
        cmd.Parameters.AddWithValue("@schema", schema);
        await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
    }

    //-----------------------//

    private static async Task ExecuteSqlBatches(SqlConnection conn, string sql)
    {
        // split on lines that contain only GO (case-insensitive)
        var batches = Regex.Split(sql, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
        foreach (var batch in batches)
        {
            var trimmed = batch.Trim();
            if (string.IsNullOrEmpty(trimmed)) continue;

            await using var cmd = new SqlCommand(trimmed, conn)
            {
                CommandTimeout = 60 * 5
            };
            await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
        }
    }

}//Cls
