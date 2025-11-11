using ID.Application.Jobs.Abstractions;
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
        ICronBuilder cron = provider.GetRequiredService<ICronBuilder>();

        provider.BuildJobStarter<ProcessMyIdOutboxMsgJob>()
           .SetupRecurringProduction(handler => handler.HandleAsync(), cron.MinuteInterval(2))
           .SetupRecurringDevelopment(handler => handler.HandleAsync(), cron.MinuteInterval(5));


        provider.BuildJobStarter<Process_Old_MyIdOutboxMsgs>()
            .SetupRecurringProduction(handler => handler.HandleAsync(), cron.Weekly())
            .SetupRecurringDevelopment(handler => handler.HandleAsync(), cron.Monthly());


        return provider;
    }


}//Cls
