using ID.IntegrationEvents.Abstractions;
using MassTransit;
using MassTransit.Configuration;
using MassTransit.Metadata;
using MassTransit.Util;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace ID.IntegrationEvents.Setup.MT;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "May use IntegrationEventsOptions late in more complicated setups")]
/// <summary>
///  Setup Event pub/sub with MassTransit
/// </summary>
internal static class EventSetupMT
{

    //------------------------//

    /// <summary>
    /// Setup Event pub/sub with MassTransit
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddEventsMT(
        this IServiceCollection services, IntegrationEventsOptions options)
    {
        services.TryAddScoped<IEventBus, EventBusMT>();

        if (options.UseSeperateEventBus)
            services.RegisterServicesMassTransitWithSeparateBus(options);
        else
            services.RegisterServicesMassTransit(options);


        return services;
    }

    //- - - - - - - - - - - - //


    private static IServiceCollection RegisterServicesMassTransit(
        this IServiceCollection services, IntegrationEventsOptions options)
    {

        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();
            busConfigurator.UsingInMemory((ctx, config) => config.ConfigureEndpoints(ctx));
            busConfigurator.AddConsumers(typeof(IdIntegrationEventsAssemblyReference).Assembly);
        });

        return services;
    }

    //- - - - - - - - - - - - //

    private static IServiceCollection RegisterServicesMassTransitWithSeparateBus(
        this IServiceCollection services, IntegrationEventsOptions options)
    {

        services.AddMassTransit<IMyIdMtBus>(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();
            busConfigurator.UsingInMemory((ctx, config) => config.ConfigureEndpoints(ctx));
            busConfigurator.AddConsumers(typeof(IdIntegrationEventsAssemblyReference).Assembly);
        });

        return services;
    }

    //------------------------//

    public static IServiceCollection RegisterListenersMT(this IServiceCollection services, params Assembly[] assemblies)
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

    //------------------------//

}
