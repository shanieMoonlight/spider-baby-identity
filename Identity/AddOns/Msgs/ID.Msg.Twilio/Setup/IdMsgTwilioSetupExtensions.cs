using ID.Application.AppAbs.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ID.Msg.Twilio.Setup;

public static class IdMsgTwilioSetupExtensions
{

    /// <summary>
    /// Setup IdMsgTwilio with lambda configuration (legacy compatibility).
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="config">Action to configure the options.</param>
    /// <returns>The same services</returns>
    public static IServiceCollection AddMyIdMessagingTwilio(this IServiceCollection services, Action<IdMsgTwilioOptions> config)
    {
        var setupOptions = new IdMsgTwilioOptions();
        config(setupOptions);

        return services.AddMyIdMessagingTwilio(setupOptions);
    }

    //------------------------------------//    

    /// <summary>
    /// Setup IdMsgTwilio with an options object.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="twilioOptions">The Twilio options to configure.</param>
    /// <returns>The same services</returns>
    public static IServiceCollection AddMyIdMessagingTwilio(this IServiceCollection services, 
        IdMsgTwilioOptions twilioOptions)
    {
        services.ConfigureTwilioOptions(twilioOptions);

        services.Replace(ServiceDescriptor.Transient<IIdSmsService, IdTwilioSmsService>());
        services.Replace(ServiceDescriptor.Transient<IIdWhatsAppService, IdTwilioWhatsAppService>());

        return services;
    }

    //------------------------------------//

    /// <summary>
    /// Setup IdMsgTwilio with configuration binding.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration instance to bind from.</param>
    /// <param name="sectionName">Optional section name. If null, assumes configuration is already the Twilio section.</param>
    /// <returns>The same services</returns>
    public static IServiceCollection AddMyIdMessagingTwilio(this IServiceCollection services, 
        IConfiguration configuration, 
        string? sectionName = null)
    {
        services.ConfigureTwilioOptions(configuration, sectionName);

        services.Replace(ServiceDescriptor.Transient<IIdSmsService, IdTwilioSmsService>());
        services.Replace(ServiceDescriptor.Transient<IIdWhatsAppService, IdTwilioWhatsAppService>());

        return services;
    }

    //------------------------------------//   


}//Cls

