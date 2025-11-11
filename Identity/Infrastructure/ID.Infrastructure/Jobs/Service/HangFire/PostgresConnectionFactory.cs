using System.Data.Common;
using Npgsql;
using Hangfire.PostgreSql; // adjust if interface type is in a different namespace

namespace ID.Infrastructure.Jobs.Service.HangFire;

internal class PostgresConnectionFactory(string connectionString) : IConnectionFactory // adjust interface name if needed
{

    // Most IConnectionFactory-like interfaces expect a DbConnection or IDbConnection
    public DbConnection Create() => new NpgsqlConnection(connectionString);

    public NpgsqlConnection GetOrCreateConnection() =>
        // Return a new connection instance. PostgreSqlStorage will open and manage it.
        new(connectionString);

}//Cls