using Hangfire;
using ID.GlobalSettings.Jobs.DbMntc;

namespace ID.Application.Jobs.DbMntc;

public static class DbMntcJobsStarterExtensions
{

    public static IServiceProvider StartDbMntcJobs(this IServiceProvider provider, CancellationToken cancellationToken)
    {
        provider.BuildJobStarter<ATeamSubscriptionCheckJob>()
            .SetupRecurringProduction(handler => handler.HandleAsync(cancellationToken), Cron.Daily())
            .SetupRecurringDevelopment(handler => handler.HandleAsync(cancellationToken), Cron.Daily());


        provider.BuildJobStarter<ATeamLeaderMntcJob>()
            .SetupRecurringProduction(handler => handler.HandleAsync(cancellationToken), Cron.Daily())
            .SetupRecurringDevelopment(handler => handler.HandleAsync(cancellationToken), Cron.Daily());

        provider.BuildJobStarter<AOldRefreshTokensJob>()
            .SetupRecurringProduction(handler => handler.HandleAsync(cancellationToken), Cron.Weekly())
            .SetupRecurringDevelopment(handler => handler.HandleAsync(cancellationToken), Cron.Daily());

        return provider;
    }


}//Cls
