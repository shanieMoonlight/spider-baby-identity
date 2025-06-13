using ID.IntegrationEvents.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace ID.IntegrationEvents.Setup.Mdtr;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "May use IntegrationEventsOptions late in more complicated setups")]
internal static class EventSetupMdtr
{
    //------------------------//

    /// <summary>
    /// Setup Event pub/sub with Mediatr
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddEventsMdtr(this IServiceCollection services, IntegrationEventsOptions options)
    {
        services.TryAddScoped<IEventBus, EventBusMdtr>();
        return services.AddMediatR(config =>
            config.RegisterServicesFromAssembly(typeof(IdIntegrationEventsAssemblyReference).Assembly));
    }

    //------------------------//

    public static IServiceCollection RegisterListenersMdtr(this IServiceCollection services, params Assembly[] assemblies) =>
        services.AddMediatR(config => config.RegisterServicesFromAssemblies(assemblies));


}//Cls
