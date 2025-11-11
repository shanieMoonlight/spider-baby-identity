using Hangfire;
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
        provider.BuildJobStarter<TeamSubscriptionCheckJob>()
            .SetupRecurringProduction(handler => handler.HandleAsync(cancellationToken), Cron.Daily())
            .SetupRecurringDevelopment(handler => handler.HandleAsync(cancellationToken), Cron.Daily());


        provider.BuildJobStarter<TeamLeaderMntcJob>()
            .SetupRecurringProduction(handler => handler.HandleAsync(cancellationToken), Cron.Daily())
            .SetupRecurringDevelopment(handler => handler.HandleAsync(cancellationToken), Cron.Daily());

        provider.BuildJobStarter<OldRefreshTokensJob>()
            .SetupRecurringProduction(handler => handler.HandleAsync(cancellationToken), Cron.Weekly())
            .SetupRecurringDevelopment(handler => handler.HandleAsync(cancellationToken), Cron.Daily());

        return provider;
    }


}//Cls
