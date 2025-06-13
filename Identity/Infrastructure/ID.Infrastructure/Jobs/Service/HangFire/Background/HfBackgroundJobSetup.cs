using ID.Infrastructure.Jobs.Service.HangFire.Background.Mgrs.Abs;
using ID.Infrastructure.Jobs.Service.HangFire.Background.Mgrs.Imps;
using Microsoft.Extensions.DependencyInjection;

namespace ID.Infrastructure.Jobs.Service.HangFire.Background;
internal static class BackgroundJobSetup
{
    /// <summary>
    /// Registers the various Hangfire background job managers in the service collection.
    /// </summary>
    /// <remarks>This method adds scoped instances of <see cref="IHfDefaultBackgroundJobMgr"/> and <see
    /// cref="IHfPriorityBackgroundJobMgr"/> to the service collection, enabling their use in Hangfire-based background
    /// job processing.</remarks>
    /// <param name="services">The <see cref="IServiceCollection"/> to which the background job managers will be added.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection AddMyIdHangfireBackgroundJobs(this IServiceCollection services)
    {
        services.AddScoped<IHfDefaultBackgroundJobMgr, HfDefaultBackgroundJobMgr>();
        services.AddScoped<IHfPriorityBackgroundJobMgr, HfPriorityBackgroundJobMgr>();
        return services;
    }

}
