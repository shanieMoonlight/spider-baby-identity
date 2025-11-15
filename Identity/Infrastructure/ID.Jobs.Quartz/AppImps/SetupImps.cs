using ID.Application.Jobs.Abstractions;
using ID.Jobs.Quartz.AppImps.JobService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ID.Jobs.Quartz.AppImps;
internal static class SetupImps
{

    public static IServiceCollection AddQuartzAppImplementations(this IServiceCollection services)
    {
        // ensure handler adapter open-generic is registered
        services.AddTransient(typeof(HandlerAdapter<>));

        services.TryAddScoped<ICronBuilder, QuartzCronBuilder>();
        services.TryAddSingleton<IMyIdJobService, QuartzMyIdJobService>();

        services.TryAddScoped<IJobsDbMigrator, QuartzDbMigrator>();

        return services;
    }



}//Cls
