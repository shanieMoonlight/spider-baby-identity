using ID.Application.Jobs.Abstractions;
using ID.Application.Jobs.DbMntc;
using ID.Application.Jobs.OutboxMsgs;
using Microsoft.Extensions.DependencyInjection;

namespace ID.Application.Jobs;

public static class MyIdJobSetupExtensions
{

    public static IServiceCollection AddRecurringMyIdJobs(this IServiceCollection services) => 
        services
            .AddOutboxMsgJobs()
            .AddDbMntcJobs();

    //---------------------//

    public static void StartRecurringMyIdJobs(this IServiceProvider provider)
    {
        using var scope = provider.CreateScope();
        var scopedProvider = scope.ServiceProvider;
        var jobService = scopedProvider.GetRequiredService<IMyIdJobService>();

        //List job starters here
        scopedProvider
            .StartOutboxJobs()
            .StartDbMntcJobs();
    }

}//Cls
