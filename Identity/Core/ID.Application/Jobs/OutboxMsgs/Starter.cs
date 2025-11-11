using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ID.Application.Jobs.OutboxMsgs;

public static class OutboxJobsStarterExtensions
{
    public static IServiceCollection AddOutboxMsgJobs(this IServiceCollection services)
    {
        services.TryAddScoped<ProcessMyIdOutboxMsgJob>();
        services.TryAddScoped<Process_Old_MyIdOutboxMsgs>();
        return services;
    }


    //---------------------//


    public static IServiceProvider StartOutboxJobs(this IServiceProvider provider, CancellationToken cancellationToken)
    {
        provider.BuildJobStarter<ProcessMyIdOutboxMsgJob>()
           .SetupRecurringProduction(handler => handler.HandleAsync(), "*/2 * * * *")
           .SetupRecurringDevelopment(handler => handler.HandleAsync(), "*/5 * * * *");


        provider.BuildJobStarter<Process_Old_MyIdOutboxMsgs>()
            .SetupRecurringProduction(handler => handler.HandleAsync(), Cron.Weekly())
            .SetupRecurringDevelopment(handler => handler.HandleAsync(), Cron.Monthly());


        return provider;
    }


}//Cls
