using ID.IntegrationEvents.Setup;
using ID.PhoneConfirmation.Events.Integration.Bus;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace ID.PhoneConfirmation.Setup;

public static class IdPhoneConfirmationSetupExtensions
{
    //No options to configure yet
    ///// <summary>
    ///// Configures SMTP options from IConfiguration and validates them.
    ///// </summary>
    //public static IServiceCollection AddMyIdPhoneConfirmation(
    //    this IServiceCollection services, IConfiguration configuration, string? sectionName = null, Assembly? customConsumerAssembly = null)
    //{
    //    ArgumentNullException.ThrowIfNull(configuration);

    //    // If sectionName provided, get that section; otherwise assume configuration is already the JWT section
    //    var configSection = string.IsNullOrWhiteSpace(sectionName)
    //        ? configuration
    //        : configuration.GetSection(sectionName);

    //    // Register options binding and validate on start using validator
    //    services
    //        .AddOptionsWithValidateOnStart<IdPhoneConfirmationSetupOptions, IdPhoneConfirmationSetupOptionsValidator>()
    //        .Bind(configSection);

    //    return services.ConfigureServices(customConsumerAssembly);
    //}

    //-----------------------//

    /// <summary>
    /// Setup ID.PhoneConfirmation
    /// </summary>
    /// <param name="services">Specifies the contract for a collection of service descriptors.</param>
    /// <param name="customConsumerAssembly">Assembley where custom listeners/consumers live</param>
    /// <returns>The same services</returns>
    public static IServiceCollection AddMyIdPhoneConfirmation(this IServiceCollection services, Assembly? customConsumerAssembly = null)
    {
        //No options to configure yet
        //services
        //    .AddOptionsWithValidateOnStart<IdPhoneConfirmationSetupOptions, IdPhoneConfirmationSetupOptionsValidator>()
        //    .Configure(config);

        return services.ConfigureServices(customConsumerAssembly);

    }

    //-----------------------//

    private static IServiceCollection ConfigureServices(this IServiceCollection services, Assembly? customConsumerAssembly)
    {

        var assembly = customConsumerAssembly ?? typeof(IdPhoneConfirmationAssemblyReference).Assembly;

        services.TryAddScoped<IPhoneConfirmationBus, PhoneConfirmationBus>();

        //Domain Events
        services.AddMediatR(config =>
            config.RegisterServicesFromAssembly(assembly)
        );

        //Integration Events
        services.RegisterIdEventListeners(assembly);


        services.AddControllers()
            .PartManager.ApplicationParts.Add(new AssemblyPart(typeof(IdPhoneConfirmationAssemblyReference).Assembly));

        return services;
    }

}//Cls
