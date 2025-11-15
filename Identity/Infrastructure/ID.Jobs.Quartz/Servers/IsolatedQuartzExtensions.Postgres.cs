using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace ID.Jobs.Quartz.Servers;

internal static class IsolatedQuartzExtensionsPostgres
{
    internal static IServiceCollection AddIsolatedQuartz_Postgres(
        this IServiceCollection services, string connectionString, string schema, string tablePrefix)
    {
        services.AddQuartz(q =>
        {
            q.SchedulerName = QuartzConstants.Scheduler;

            q.UsePersistentStore(storeBuilder =>
            {
                storeBuilder.UseProperties = true;
                storeBuilder.RetryInterval = TimeSpan.FromSeconds(15);
                storeBuilder.UseNewtonsoftJsonSerializer();

                storeBuilder.PerformSchemaValidation = false;

                // Build connection string that sets the search_path so unqualified names land in `schema`
                // Requires NpgsqlConnectionStringBuilder from Npgsql (add Npgsql package if missing)
                var csb = new Npgsql.NpgsqlConnectionStringBuilder(connectionString)
                {
                    SearchPath = $"{schema},public"
                };

                // Use the Postgres provider method your Quartz package exposes.
                // Method name may be `UsePostgres`, `UsePostgreSql`, or `UseNpgsql` depending on version.
                storeBuilder.UsePostgres(sqlOptions =>
                {
                    sqlOptions.ConnectionString = csb.ConnectionString;
                    // For Postgres prefer tablePrefix without schema because search_path handles schema resolution
                    sqlOptions.TablePrefix = tablePrefix;
                    // If provider exposes Schema property you can set it, otherwise use SearchPath above
                    // try { var prop = sqlOptions.GetType().GetProperty("Schema"); if (prop != null && prop.CanWrite) prop.SetValue(sqlOptions, schema); } catch {}
                });
            });
        });

        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
        return services;
    }

}




