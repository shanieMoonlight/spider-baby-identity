using ID.Domain.Utility.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ID.IntegrationEvents.Setup;
internal static class IntegrationEventsOptionsSetup
{
    /// <summary>
    /// Configures IdIntegrationEvents options with direct parameter values.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="sgOptions">The IdIntegrationEvents options to configure.</param>
    /// <returns>The service collection for method chaining.</returns>
    internal static IServiceCollection ConfigureIdIntegrationEventsOptions(this IServiceCollection services, IntegrationEventsOptions sgOptions)
    {
        ValidateIdIntegrationEventsOptions(sgOptions);

        // Configure IdIntegrationEvents-specific options
        services.Configure<IntegrationEventsOptions>(opts =>
        {
            opts.Provider = sgOptions.Provider;
            opts.UseInMemory = sgOptions.UseInMemory;
            opts.ConnectionString = sgOptions.ConnectionString;
            opts.QueuePrefix = sgOptions.QueuePrefix;
            opts.MessageTimeout = sgOptions.MessageTimeout;
            opts.UseSeperateEventBus = sgOptions.UseSeperateEventBus;
        });

        return services;
    }

    //------------------------//

    /// <summary>
    /// Configures IdIntegrationEvents options by binding from configuration.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">The configuration instance to bind from. Can be root config or a specific section.</param>
    /// <param name="sectionName">Optional section name. If null, assumes configuration is already the IdIntegrationEvents section.</param>
    /// <returns>The service collection for method chaining.</returns>
    internal static IServiceCollection ConfigureIdIntegrationEventsOptions(
        this IServiceCollection services, IConfiguration configuration, string? sectionName = null)
    {
        if (configuration == null)
            throw new SetupDataException(nameof(IntegrationEventsOptions), nameof(configuration));

        // If sectionName provided, get that section; otherwise assume configuration is already the JWT section
        var configSection = string.IsNullOrWhiteSpace(sectionName)
            ? configuration
            : configuration.GetSection(sectionName);

        services.Configure<IntegrationEventsOptions>(configSection);


        return services;
    }

    //------------------------//

    private static void ValidateIdIntegrationEventsOptions(IntegrationEventsOptions options)
    {
    }

}
