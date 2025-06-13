using ID.IntegrationEvents.Setup;
using ID.PhoneConfirmation.Events.Integration.Bus;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace ID.PhoneConfirmation.Setup;

public static class IdPhoneConfirmationSetupExtensions
{
    //-------------------------------------//

    /// <summary>
    /// Setup ID.PhoneConfirmation
    /// </summary>
    /// <param name="services">Specifies the contract for a collection of service descriptors.</param>
    /// <param name="customConsumerAssembly">Assembley where custom listeners/consumers live</param>
    /// <returns>The same services</returns>
    public static IServiceCollection AddMyIdPhoneConfirmation(this IServiceCollection services,
        IdPhoneConfirmationSetupOptions setupOptions,
        Assembly? customConsumerAssembly = null)
    {

        services.ConfigureIdPhoneConfirmationSetupOptions(setupOptions);

        services.ConfigureServices(customConsumerAssembly);

        return services;

    }

    //-------------------------------------//

    /// <summary>
    /// Setup ID.PhoneConfirmation
    /// </summary>
    /// <param name="services">Specifies the contract for a collection of service descriptors.</param>
    /// <param name="customConsumerAssembly">Assembley where custom listeners/consumers live</param>
    /// <returns>The same services</returns>
    public static IServiceCollection AddMyIdPhoneConfirmation(this IServiceCollection services,
        Action<IdPhoneConfirmationSetupOptions>? config = null,
        Assembly? customConsumerAssembly = null)
    {
        IdPhoneConfirmationSetupOptions setupOptions = new();
        config?.Invoke(setupOptions);

        return services.AddMyIdPhoneConfirmation(setupOptions, customConsumerAssembly);

    }

    //-------------------------------------//

    private static void ConfigureServices(this IServiceCollection services, Assembly? customConsumerAssembly)
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
    }

    //-------------------------------------//

}//Cls
