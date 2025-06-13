using ID.Domain.Utility.Exceptions;
using ID.IntegrationEvents.Setup.Mdtr;
using ID.IntegrationEvents.Setup.MT;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ID.IntegrationEvents.Setup;

/// <summary>
///  Setup Event pub/sub
/// </summary>
public static class EventSetup
{
    public static IServiceCollection AddMyIdEvents(
        this IServiceCollection services, IntegrationEventsOptions? options = null)
    {
        options ??= new IntegrationEventsOptions();
        services.ConfigureIdIntegrationEventsOptions(options);


        if (options.Provider == EventBusProvider.MassTransit)
            return services.AddEventsMT(options);
        else if (options.Provider == EventBusProvider.MediatR)
            return services.AddEventsMdtr(options);
        else
            throw new MyIdException($"{nameof(EventBusProvider)} '{options.Provider}' is not supported.");

    }

    //------------------------//

    public static IServiceCollection RegisterIdEventListeners(this IServiceCollection services, params Assembly[] assemblies) =>
        services.RegisterListenersMT(assemblies);


}//Cls
