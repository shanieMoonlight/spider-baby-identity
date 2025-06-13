using ID.Infrastructure.Jobs.Service.HangFire.Recurring.Mgrs.Abs;
using ID.Infrastructure.Jobs.Service.HangFire.Recurring.Mgrs.Imps;
using ID.Infrastructure.Setup;
using Microsoft.Extensions.DependencyInjection;

namespace ID.Infrastructure.Jobs.Service.HangFire.Recurring;
internal static class HfRecurringJobSetup
{
    /// <summary>
    /// Registers recurring job managers for Hangfire in the service collection.
    /// </summary>
    /// <remarks>This method adds scoped services for managing  recurring jobs in various queues in
    /// Hangfire. It is intended to be used as part of the application's dependency injection setup.</remarks>
    /// <param name="services">The <see cref="IServiceCollection"/> to which the recurring job managers will be added.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection AddMyIdHangfireRecurringJobs(this IServiceCollection services)
    {
        services.AddScoped<IHfDefaultRecurringJobMgr, HfDefaultRecurringJobMgr>();
        services.AddScoped<IHfPriorityRecurringJobMgr, HfPriorityRecurringJobMgr>();
        return services;
    }

}
