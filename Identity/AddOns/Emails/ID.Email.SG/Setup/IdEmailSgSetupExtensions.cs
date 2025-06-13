using ID.Email.Base.Setup;
using ID.Email.SG.Service;
using ID.IntegrationEvents.Setup;
using Microsoft.Extensions.DependencyInjection;

namespace ID.Email.SG.Setup;

public static class IdEmailSgSetupExtensions
{


    /// <summary>
    /// Setup MyId SendGrid Email
    /// </summary>
    /// <param name="services"></param>
    /// <returns>The same services</returns>
    public static IServiceCollection AddMyIdEmailSG(this IServiceCollection services, IdEmailSgOptions setupOptions)
    {

        services.AddIdEmailBase<IdEmailSgService>(setupOptions);
        services.RegisterIdEventListeners(typeof(IdEmailSgAssemblyReference).Assembly);

        services.ConfigureSendGridOptions(setupOptions);

        return services;

    }

    //-------------------------------------//

    /// <summary>
    /// Setup MyId SendGrid Email
    /// </summary>
    /// <param name="services"></param>
    /// <returns>The same services</returns>
    public static IServiceCollection AddMyIdEmailSG(this IServiceCollection services, Action<IdEmailSgOptions> config)
    {
        IdEmailSgOptions setupOptions = new();
        config(setupOptions);

        return services.AddMyIdEmailSG(setupOptions);

    }

}//Cls

