using ID.Application.Jobs.Abstractions;
using ID.Application.Models;
using ID.Jobs.Quartz.Persistence.Initializers.SQL;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ID.Jobs.Quartz;
public static class Setup
{

    public static IServiceCollection AddMyIdQuartzJobs(
        this IServiceCollection services,
        DatabaseType databaseType,
        string connectionString,
        bool ensureDb = true)
    {
        if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentNullException(nameof(connectionString));

        // Optionally ensure DB objects exist (opt-in)
        if (ensureDb)
        {
            // default embedded resource name from initializer
            QuartzDbInitializer_SQL.EnsureQuartzSchemaAsyncFromEmbeddedResource(connectionString)
                .GetAwaiter().GetResult();
        }   

        services.AddScoped<ICronBuilder, QuartzCronBuilder>();

        services.AddMyIdIsolatedQuartz(connectionString, schema: QuartzConstants.SCHEMA);

        // ensure handler adapter open-generic is registered
        services.AddTransient(typeof(HandlerAdapter<>));

        return services;
    }



}//Cls
