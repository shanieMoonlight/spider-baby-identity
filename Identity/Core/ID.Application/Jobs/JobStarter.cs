using ID.Application.Jobs.Abstractions;
using ID.Application.Jobs.DbMntc;
using ID.Application.Jobs.OutboxMsgs;
using Microsoft.Extensions.DependencyInjection;

namespace ID.Application.Jobs;

public static class MyIdJobSetupExtensions
{

    public static void StartRecurringMyIdJobs(this IServiceProvider provider, CancellationToken cancellationToken)
    {
        using var scope = provider.CreateScope();
        var scopedProvider = scope.ServiceProvider;
        var jobService = scopedProvider.GetRequiredService<IMyIdJobService>();

        //List job starters here
        scopedProvider
            .StartOutboxJobs(cancellationToken)
            .StartDbMntcJobs(cancellationToken);
    }

}//Cls
