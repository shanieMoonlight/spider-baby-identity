using MassTransit;
using MassTransit.Configuration;
using MassTransit.Metadata;
using MassTransit.Util;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MyIdDemo.Setup.Events.MT.Abstractions;
using System.Reflection;

namespace MyIdDemo.Setup.Events.MT;

/// <summary>
///  Setup Event pub/sub with MassTransit
/// </summary>
internal static class EventSetupMT
{
    //------------------------------------//

    /// <summary>
    /// Setup Event pub/sub with MassTransit
    /// </summary>
    internal static IServiceCollection AddEventsMT(this IServiceCollection services, Assembly assembly)
    {
        services.TryAddScoped<IMyIdDemoEventBus, EventBusMT>();

        try
        {
            services.AddMassTransit(busConfigurator =>
            {
                busConfigurator.SetKebabCaseEndpointNameFormatter();
                busConfigurator.UsingInMemory((ctx, config) => config.ConfigureEndpoints(ctx));
                busConfigurator.AddConsumers(assembly);
            });
        }
        catch (ConfigurationException)
        {
            return services.AddMTWithCustomBus(assembly);
        }

        return services;
    }

    //------------------------------------//

    /// <summary>
    /// Setup Event pub/sub with MassTransit
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddMTWithCustomBus(this IServiceCollection services, Assembly assembly)
    {

        services.AddMassTransit<IMyIdDemoMtBus>(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();
            busConfigurator.UsingInMemory((ctx, config) => config.ConfigureEndpoints(ctx));
            busConfigurator.AddConsumers(assembly);
        });

        return services;
    }

    //------------------------------------//

    internal static IServiceCollection RegisterListenersMT(this IServiceCollection services, params Assembly[] assemblies)
    {
        var registrar = new DependencyInjectionContainerRegistrar(services);

        var types = AssemblyTypeCache.FindTypes(assemblies, RegistrationMetadata.IsConsumerOrDefinition)
            .GetAwaiter()
            .GetResult();

        foreach (var type in types.FindTypes(TypeClassification.Concrete | TypeClassification.Closed).ToArray())
        {
            services.RegisterConsumer(registrar, type);
        }

        return services;
    }

    //------------------------------------//
}
