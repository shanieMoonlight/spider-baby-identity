using ID.Application.AppAbs.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ID.API.Setup.ExtraSetup;

public static class IdMsgSetupExtensions
{
    //----------------------------------------//

    /// <summary>
    /// Setup Sms using your own implementation of IIdSmsService
    /// </summary>
    /// <returns>The same services</returns>
    public static IServiceCollection AddCustomSmsMessaging<TSmsService>(this IServiceCollection services) where TSmsService : class, IIdSmsService =>
        services.Replace(ServiceDescriptor.Transient<IIdSmsService, TSmsService>());

    //- - - - - - - - - - - - - - - - - - - - //

    /// <summary>
    /// Setup WhatsApp Messages using your own implementation of IIdWhatsAppService
    /// </summary>
    /// <returns>The same services</returns>
    public static IServiceCollection AddCustomWhatsappMessaging<TWhatsAppService>(this IServiceCollection services) where TWhatsAppService : class, IIdWhatsAppService =>
        services.Replace(ServiceDescriptor.Transient<IIdWhatsAppService, TWhatsAppService>());

    //----------------------------------------//

}//Cls