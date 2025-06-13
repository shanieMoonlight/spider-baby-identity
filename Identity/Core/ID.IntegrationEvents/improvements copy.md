What about the consuming application. If MyId registers first will this cause an issue for them. In my other apps that imported the originaly MyId system, I used to give them a try catch around their masstransit setup as well. That feels like a lot to ask a user of the MyId System. They'll be confused. Is their a way to use IMyIdMtBus from the start AND have the published message arrive at the consumer?

GitHub Copilot
Excellent point! You're absolutely right - requiring users to handle try/catch around their MassTransit setup is a terrible developer experience. Let me solve this properly.

🎯 The Real Solution: Shared Transport Configuration
The key insight is that both buses need to use the SAME transport configuration for messages to flow between them.

Option 1: Transport Bridge Pattern (Recommended)


public static IServiceCollection AddEventsMT(
    this IServiceCollection services, IntegrationEventsOptions options)
{
    services.TryAddScoped<IEventBus, EventBusMT>();

    // ALWAYS use IMyIdMtBus - but configure it to bridge to main transport
    services.AddMassTransit<IMyIdMtBus>(busConfigurator =>
    {
        busConfigurator.SetKebabCaseEndpointNameFormatter();
        
        // Configure to use same transport as main app will use
        busConfigurator.UsingInMemory((ctx, config) => 
        {
            // Key: Use consistent transport settings
            config.ConfigureEndpoints(ctx);
            
            // Register consumers on this bus
            config.ReceiveEndpoint("myid-events", e =>
            {
                e.ConfigureConsumers(ctx);
            });
        });
        
        busConfigurator.AddConsumers(typeof(IdIntegrationEventsAssemblyReference).Assembly);
    });

    return services;
}




Option 2: Consumer Registration Strategy

public static IServiceCollection AddEventsMT(
    this IServiceCollection services, IntegrationEventsOptions options)
{
    services.TryAddScoped<IEventBus, EventBusMT>();

    // Always use separate bus for publishing
    services.AddMassTransit<IMyIdMtBus>(busConfigurator =>
    {
        busConfigurator.SetKebabCaseEndpointNameFormatter();
        busConfigurator.UsingInMemory((ctx, config) => config.ConfigureEndpoints(ctx));
        
        // DON'T register consumers here - let host app register them
    });

    // Register a service that adds consumers to the main bus if it exists
    services.AddSingleton<IHostedService, MyIdConsumerRegistrationService>();

    return services;
}

// This service registers your consumers on whatever bus the host app creates
public class MyIdConsumerRegistrationService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    
    public MyIdConsumerRegistrationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Check if host app has IBus configured
        var mainBus = _serviceProvider.GetService<IBus>();
        if (mainBus != null)
        {
            // Register consumers dynamically
            // This is complex but possible
        }
        
        return Task.CompletedTask;
    }
    
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

Option 3: Configuration Delegate Pattern (Best UX)

public static IServiceCollection AddEventsMT(
    this IServiceCollection services, 
    IntegrationEventsOptions options,
    Action<IBusRegistrationConfigurator>? hostBusConfiguration = null)
{
    services.TryAddScoped<IEventBus, EventBusMT>();

    if (hostBusConfiguration != null)
    {
        // Host app will configure MassTransit - we just add our consumers
        services.Configure<MassTransitHostOptions>(opts =>
        {
            opts.WaitUntilStarted = true;
        });
        
        // Add our consumers to whatever bus the host creates
        services.AddScoped<IMyIdConsumerRegistry, MyIdConsumerRegistry>();
        
        // Use IBus (host's bus) for publishing
        services.TryAddScoped<IEventBus>(provider => 
            new EventBusMT(provider.GetRequiredService<IBus>()));
    }
    else
    {
        // No host bus - create our own
        services.AddMassTransit<IMyIdMtBus>(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();
            busConfigurator.UsingInMemory((ctx, config) => config.ConfigureEndpoints(ctx));
            busConfigurator.AddConsumers(typeof(IdIntegrationEventsAssemblyReference).Assembly);
        });
        
        services.TryAddScoped<IEventBus>(provider => 
            new EventBusMT(provider.GetRequiredService<IMyIdMtBus>()));
    }

    return services;
}


🏆 My Recommended Solution: Extension Method Pattern

// In your library
public static class MassTransitExtensions
{
    public static IBusRegistrationConfigurator AddMyIdConsumers(
        this IBusRegistrationConfigurator configurator)
    {
        configurator.AddConsumers(typeof(IdIntegrationEventsAssemblyReference).Assembly);
        return configurator;
    }
}

// Your AddEventsMT becomes simple
public static IServiceCollection AddEventsMT(
    this IServiceCollection services, IntegrationEventsOptions options)
{
    services.TryAddScoped<IEventBus, EventBusMT>();
    
    // Try to register minimal bus first
    var massTransitExists = services.Any(s => s.ServiceType == typeof(IBus));
    
    if (!massTransitExists)
    {
        // No MassTransit yet - add minimal setup
        services.AddMassTransit(configurator =>
        {
            configurator.AddMyIdConsumers(); // Extension method
            configurator.UsingInMemory((ctx, config) => config.ConfigureEndpoints(ctx));
        });
    }
    
    // Always use IBus for publishing (works with host's bus or ours)
    services.TryAddScoped<IEventBus>(provider => 
        new EventBusMT(provider.GetRequiredService<IBus>()));

    return services;
}