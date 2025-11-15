using ID.Jobs.Quartz;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

internal class SqlDbConnectionFactory(IOptions<QuartzConfig> configProvider) : IDbConnectionFactory<SqlConnection>
{
    public SqlConnection CreateConnection() =>
        new(configProvider.Value.ConnectionString);
}

