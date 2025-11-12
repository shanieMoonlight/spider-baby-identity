using ID.Application.Jobs.Abstractions;
using ID.Application.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ID.Jobs.Quartz;
public static class Setup
{

    public static IServiceCollection AddMyIdQuartzJobs(this IServiceCollection services, DatabaseType databaseType, string connectionString, string schema = "myid_qtz")
    {
        services.AddScoped<ICronBuilder, QuartzCronBuilder>();
        services.AddMyIdIsolatedQuartz(connectionString, schema: schema);

        //services.AddHangfire(x=> { });
        return services;


    }
}//Cls
