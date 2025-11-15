using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace ID.Jobs.Quartz.Servers;

internal static class IsolatedQuartzExtensionsSql
{
    internal static IServiceCollection AddMyIdIsolatedQuartz_Sql(
        this IServiceCollection services, string connectionString, string schema, string tablePrefix)
    {
        services.AddQuartz(q =>
        {
            q.SchedulerName = QuartzConstants.Scheduler;



            q.UsePersistentStore(storeBuilder =>
            {
                storeBuilder.UseProperties = true;
                storeBuilder.RetryInterval = TimeSpan.FromSeconds(15);

                //When app starts there will be no schema in DB yet, so skip validation to allow Quartz to create objects
                //User will Migrate the schema separately using migrations endpoint
                storeBuilder.PerformSchemaValidation = false;

                // set clustering via storeBuilder.UseClustering() if available
                storeBuilder.UseNewtonsoftJsonSerializer();

                storeBuilder.UseSqlServer(sqlOptions =>
                {
                    sqlOptions.ConnectionString = connectionString;
                    sqlOptions.TablePrefix = $"{schema}.{tablePrefix}";
                });
            });
        });

        return services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
    }

}//Cls




