using ID.Jobs.Quartz.Persistence.Abs;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Data;
using System.Data.Common;

namespace ID.Jobs.Quartz.Persistence.Ef.SqlServer;

internal class SqlDbCommandExecutor : IDbCommandExecutor
{
    private readonly QuartzConfig _config;
    private readonly Func<DbConnection> _connectionFactory;
    private DbConnection? _connection;
    private readonly SemaphoreSlim _openLock = new(1,1);

    public SqlDbCommandExecutor(IOptions<QuartzConfig> options, Func<DbConnection>? connectionFactory = null)
    {
        _config = options.Value;
        _connectionFactory = connectionFactory ?? (() => new SqlConnection(_config.ConnectionString));
    }

    //----------------------//

    public async Task EnsureOpenAsync(CancellationToken cancellationToken = default)
    {
        await _openLock.WaitAsync(cancellationToken);
        try
        {
            _connection ??= _connectionFactory();
            if (_connection.State != ConnectionState.Open)
                await _connection.OpenAsync(cancellationToken);
        }
        finally
        {
            _openLock.Release();
        }
    }

    //----------------------//

    public async Task<object?> ExecuteScalarAsync(string sql, IDictionary<string, object?>? parameters = null, CancellationToken cancellationToken = default)
    {
        await EnsureOpenAsync(cancellationToken);
        await using var cmd = CreateCommand(sql, parameters);
        return await cmd.ExecuteScalarAsync(cancellationToken);
    }

    //----------------------//

    public async Task<int> ExecuteNonQueryAsync(string sql, IDictionary<string, object?>? parameters = null, CancellationToken cancellationToken = default)
    {
        await EnsureOpenAsync(cancellationToken);
        await using var cmd = CreateCommand(sql, parameters);
        return await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

    //----------------------//

    private DbCommand CreateCommand(string sql, IDictionary<string, object?>? parameters)
    {
        if (_connection == null) 
            throw new InvalidOperationException("Connection not opened. Call EnsureOpenAsync first.");

        var cmd = _connection.CreateCommand();
        cmd.CommandText = sql;
        cmd.CommandTimeout = 0;
        if (parameters != null)
        {
            foreach (var kv in parameters)
            {
                if (string.IsNullOrWhiteSpace(kv.Key))
                    throw new ArgumentException("Parameter name cannot be null or whitespace", nameof(parameters));

                var p = cmd.CreateParameter();
                p.ParameterName = kv.Key.StartsWith('@') ? kv.Key : "@" + kv.Key;
                p.Value = kv.Value ?? DBNull.Value;
                cmd.Parameters.Add(p);
            }
        }
        return cmd;
    }
}//Cls