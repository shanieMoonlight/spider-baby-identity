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
        return services;
    }


    //---------------------//


    public static IServiceProvider StartDbMntcJobs(this IServiceProvider provider, CancellationToken cancellationToken)
    {

        ICronBuilder cron = provider.GetRequiredService<ICronBuilder>();


        provider.BuildJobStarter<TeamSubscriptionCheckJob>()
            .SetupRecurringProduction(handler => handler.HandleAsync(cancellationToken), cron.Daily())
            .SetupRecurringDevelopment(handler => handler.HandleAsync(cancellationToken), cron.Weekly());


        provider.BuildJobStarter<TeamLeaderMntcJob>()
            .SetupRecurringProduction(handler => handler.HandleAsync(cancellationToken), cron.Daily())
            .SetupRecurringDevelopment(handler => handler.HandleAsync(cancellationToken), cron.Weekly());

        provider.BuildJobStarter<OldRefreshTokensJob>()
            .SetupRecurringProduction(handler => handler.HandleAsync(cancellationToken), cron.Weekly())
            .SetupRecurringDevelopment(handler => handler.HandleAsync(cancellationToken), cron.Monthly());

        return provider;
    }


}//Cls
