using ID.Email.Base.Setup;
using ID.Email.SMTP.Service;
using ID.IntegrationEvents.Setup;
using Microsoft.Extensions.DependencyInjection;

namespace ID.Email.SMTP.Setup;

public static class IdEmailSmtpSetupExtensions
{
    /// <summary>
    /// Setup ID.EmailSmtp
    /// </summary>
    /// <param name="services"></param>
    /// <returns>The same services</returns>
    public static IServiceCollection AddMyIdEmailSmtp(this IServiceCollection services, IdEmailSmtpOptions setupOptions)
    {
        //Setup base first
        services.AddIdEmailBase<IdEmailSmtpService>(setupOptions);

        services.ConfigureEmailSmtpOptions(setupOptions);

        services.RegisterIdEventListeners(typeof(IdEmailSmtpAssemblyReference).Assembly);

        return services;
    }


    //-------------------------------------//


    /// <summary>
    /// Setup ID.EmailSmtp
    /// </summary>
    /// <param name="services"></param>
    /// <returns>The same services</returns>
    public static IServiceCollection AddMyIdEmailSmtp(this IServiceCollection services, Action<IdEmailSmtpOptions> config)
    {
        IdEmailSmtpOptions setupOptions = new();
        config(setupOptions);

        return services.AddMyIdEmailSmtp(setupOptions);
    }



}//Cls
