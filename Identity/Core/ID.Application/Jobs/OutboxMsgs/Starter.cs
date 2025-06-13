using Hangfire;

namespace ID.Application.Jobs.OutboxMsgs;

public static class OutboxJobsStarterExtensions
{

    public static IServiceProvider StartOutboxJobs(this IServiceProvider provider, CancellationToken cancellationToken)
    {
        provider.BuildJobStarter<AProcessMyIdOutboxMsgJob>()
           .SetupRecurringProduction(handler => handler.HandleAsync(), "*/2 * * * *")
           .SetupRecurringDevelopment(handler => handler.HandleAsync(), "*/5 * * * *");


        provider.BuildJobStarter<AProcess_Old_MyIdOutboxMsgs>()
            .SetupRecurringProduction(handler => handler.HandleAsync(), Cron.Weekly())
            .SetupRecurringDevelopment(handler => handler.HandleAsync(), Cron.Monthly());


        return provider;
    }


}//Cls
