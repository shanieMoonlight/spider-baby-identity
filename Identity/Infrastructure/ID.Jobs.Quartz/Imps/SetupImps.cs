using ID.Application.Jobs.Abstractions;
using ID.Jobs.Quartz.Imps.JobService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ID.Jobs.Quartz.Imps;
internal static class SetupImps
{

    public static IServiceCollection AddMyIdQuartzJobs(this IServiceCollection services)
    {
        // ensure handler adapter open-generic is registered
        services.AddTransient(typeof(HandlerAdapter<>));

        services.TryAddScoped<ICronBuilder, QuartzCronBuilder>();
        services.TryAddSingleton<IMyIdJobService, QuartzMyIdJobService>();

        return services;
    }



}//Cls
