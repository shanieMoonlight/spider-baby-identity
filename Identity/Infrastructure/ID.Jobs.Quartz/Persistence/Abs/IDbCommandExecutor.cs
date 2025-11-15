using System.Data.Common;

namespace ID.Jobs.Quartz.Persistence.Abs;

internal interface IDbCommandExecutor
{
    Task EnsureOpenAsync(CancellationToken cancellationToken = default);

    Task<object?> ExecuteScalarAsync(string sql, IDictionary<string, object?>? parameters = null, CancellationToken cancellationToken = default);

    Task<int> ExecuteNonQueryAsync(string sql, IDictionary<string, object?>? parameters = null, CancellationToken cancellationToken = default);
}