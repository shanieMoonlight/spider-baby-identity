using ID.Application.Jobs.DbMntc;
using ID.GlobalSettings.Jobs.DbMntc;
using ID.Infrastructure.Jobs.Imps.DbMntc.Jobs;
using Microsoft.Extensions.DependencyInjection;

namespace ID.Infrastructure.Jobs.Imps.DbMntc;

internal static class DbMntcSetup
{

    public static IServiceCollection AddDbMntcJobs(this IServiceCollection services)//, IConfiguration iOptionsConfig
    {
        return services
            .AddSingleton<ATeamLeaderMntcJob, TeamLeaderMntcJob>()
            .AddSingleton<AOldRefreshTokensJob, OldRefreshTokensJob>()
            .AddSingleton<ATeamSubscriptionCheckJob, TeamSubscriptionCheckJob>();
    }

}//Cls