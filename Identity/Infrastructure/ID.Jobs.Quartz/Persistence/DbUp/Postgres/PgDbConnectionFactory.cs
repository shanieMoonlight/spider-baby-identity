using ID.Jobs.Quartz;
using Microsoft.Extensions.Options;
using Npgsql;

internal class PgDbConnectionFactory(IOptions<QuartzConfig> configProvider) : IDbConnectionFactory<NpgsqlConnection>
{
    public NpgsqlConnection CreateConnection() =>
        new(configProvider.Value.ConnectionString);
}