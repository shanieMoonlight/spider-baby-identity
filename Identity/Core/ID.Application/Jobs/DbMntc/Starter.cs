using ID.Application.Jobs.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ID.Application.Jobs.DbMntc;

public static class DbMntcJobsStarterExtensions
{
    public static IServiceCollection AddDbMntcJobs(this IServiceCollection services)
    {
        services.TryAddScoped<TeamSubscriptionCheckJob>();
        services.TryAddScoped<TeamLeaderMntcJob>();
        services.TryAddScoped<OldRefreshTokensJob>();
        services.TryAddScoped<ExpiredTrustedDevicesJob>();
        return services;
    }


    //---------------------//


    public static IServiceProvider StartDbMntcJobs(this IServiceProvider provider)
    {

        ICronBuilder cron = provider.GetRequiredService<ICronBuilder>();


        provider.BuildJobStarter<TeamSubscriptionCheckJob>()
            .SetupRecurringProduction(handler => handler.HandleAsync(), cron.Daily())
            .SetupRecurringDevelopment(handler => handler.HandleAsync(), cron.Weekly());


        provider.BuildJobStarter<TeamLeaderMntcJob>()
            .SetupRecurringProduction(handler => handler.HandleAsync(), cron.Daily())
            .SetupRecurringDevelopment(handler => handler.HandleAsync(), cron.Weekly());

        provider.BuildJobStarter<OldRefreshTokensJob>()
            .SetupRecurringProduction(handler => handler.HandleAsync(), cron.Weekly())
            .SetupRecurringDevelopment(handler => handler.HandleAsync(), cron.Monthly());

        provider.BuildJobStarter<ExpiredTrustedDevicesJob>()
            .SetupRecurringProduction(handler => handler.HandleAsync(), cron.Weekly())
            .SetupRecurringDevelopment(handler => handler.HandleAsync(), cron.Monthly());

        return provider;
    }


}//Cls
