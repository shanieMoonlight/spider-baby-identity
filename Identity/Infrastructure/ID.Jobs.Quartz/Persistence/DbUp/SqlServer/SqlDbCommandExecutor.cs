using ID.Jobs.Quartz.Persistence.Abs;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Data;
using System.Data.Common;

namespace ID.Jobs.Quartz.Persistence.DbUp.SqlServer;

internal class SqlDbCommandExecutor : IDbCommandExecutor
{
    private readonly QuartzConfig _config;
    private readonly Func<DbConnection> _connectionFactory;
    private DbConnection? _connection;

    public SqlDbCommandExecutor(IOptions<QuartzConfig> options, Func<DbConnection>? connectionFactory = null)
    {
        _config = options.Value;
        _connectionFactory = connectionFactory ?? (() => new SqlConnection(_config.ConnectionString));
    }

    public async Task OpenAsync(CancellationToken cancellationToken = default)
    {
        _connection ??= _connectionFactory();
        if (_connection.State != ConnectionState.Open)
            await _connection.OpenAsync(cancellationToken);
    }

    public async Task<object?> ExecuteScalarAsync(string sql, IDictionary<string, object?>? parameters = null, CancellationToken cancellationToken = default)
    {
        await using var cmd = CreateCommand(sql, parameters);
        return await cmd.ExecuteScalarAsync(cancellationToken);
    }

    public async Task<int> ExecuteNonQueryAsync(string sql, IDictionary<string, object?>? parameters = null, CancellationToken cancellationToken = default)
    {
        await using var cmd = CreateCommand(sql, parameters);
        return await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

    private DbCommand CreateCommand(string sql, IDictionary<string, object?>? parameters)
    {
        if (_connection == null) throw new InvalidOperationException("Connection not opened. Call OpenAsync first.");
        var cmd = _connection.CreateCommand();
        cmd.CommandText = sql;
        cmd.CommandTimeout = 0;
        if (parameters != null)
        {
            foreach (var kv in parameters)
            {
                var p = cmd.CreateParameter();
                p.ParameterName = kv.Key.StartsWith('@') ? kv.Key : "@" + kv.Key;
                p.Value = kv.Value ?? DBNull.Value;
                cmd.Parameters.Add(p);
            }
        }
        return cmd;
    }
}