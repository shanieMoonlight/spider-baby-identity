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

                //When app starts there will be no schema in DB yet, so skip validation to allow Quartz to create objects
                //User will Migrate the schema separately using migrations endpoint
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

        return services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
    }

}//Cls




