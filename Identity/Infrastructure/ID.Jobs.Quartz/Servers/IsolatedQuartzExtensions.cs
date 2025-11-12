using ID.Application.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ID.Jobs.Quartz.Servers;

internal static class IsolatedQuartzExtensions
{
    /// <summary>
    /// Orchestrator that registers the isolated Quartz scheduler using the appropriate provider-specific setup.
    /// </summary>
    public static IServiceCollection AddIsolatedQuartz(
        this IServiceCollection services, DatabaseType databaseType, string connectionString, string schema, string tablePrefix)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        return databaseType switch
        {
            DatabaseType.SqlServer => IsolatedQuartzExtensionsSql.AddMyIdIsolatedQuartz_Sql(services, connectionString, schema, tablePrefix),
            DatabaseType.PostgreSql => IsolatedQuartzExtensionsPostgres.AddIsolatedQuartz_Postgres(services, connectionString, schema, tablePrefix),
            _ => throw new NotSupportedException($"Database type {databaseType} is not supported for isolated Quartz registration.")
        };
    }

}//Cls
