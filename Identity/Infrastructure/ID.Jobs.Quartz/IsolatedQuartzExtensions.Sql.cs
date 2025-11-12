using ID.Application.Jobs.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace ID.Jobs.Quartz;

internal static class IsolatedQuartzExtensionsSql
{
    internal static IServiceCollection AddMyIdIsolatedQuartz(
        this IServiceCollection services, string connectionString, string schema = "myid_qtz", string tablePrefix = "QRTZ_")
    {
        services.AddQuartz(q =>
        {
            q.SchedulerName = "MyIdQuartzScheduler";
            

            q.UsePersistentStore(storeBuilder =>
            {
                storeBuilder.UseProperties = true;
                storeBuilder.RetryInterval = TimeSpan.FromSeconds(15);
                // set clustering via storeBuilder.UseClustering() if available
                storeBuilder.UseNewtonsoftJsonSerializer();

                storeBuilder.UseSqlServer(sqlOptions =>
                {
                    sqlOptions.ConnectionString = connectionString;
                    sqlOptions.TablePrefix = $"{schema}.{tablePrefix}";
                    // schema support may vary by provider; if available set it
                    //try
                    //{
                    //    var prop = sqlOptions.GetType().GetProperty("Schema");
                    //    if (prop != null && prop.CanWrite)
                    //        prop.SetValue(sqlOptions, schema);
                    //}
                    //catch
                    //{
                    //    // ignore if provider doesn't support schema property on options
                    //}
                });
            });
        });

        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        // Generic job and service
        //services.AddTransient<GenericQuartzJob>();
        services.AddSingleton<IMyIdJobService, QuartzMyIdJobService>();

        return services;
    }
}




