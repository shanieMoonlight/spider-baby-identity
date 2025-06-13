using ID.Application.Jobs.OutboxMsgs;
using ID.Infrastructure.Jobs.Imps.OutboxMsg.Jobs;
using Microsoft.Extensions.DependencyInjection;

namespace ID.Infrastructure.Jobs.Imps.OutboxMsg;

internal static class OutboxMsgSetup
{

    public static IServiceCollection AddProcessOutboxMsgJobs(this IServiceCollection services)
    {
        return services
            .AddScoped<AProcessMyIdOutboxMsgJob, ProcessOutboxMsgJob>() //Scoped because it might indirectly consume a Scoped Service via Mediatr
            .AddSingleton<AProcess_Old_MyIdOutboxMsgs, ProcessOldOutboxMsgs>();
    }

}//Cls