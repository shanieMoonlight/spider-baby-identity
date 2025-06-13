using ID.Email.Base.Abs;
using ID.Email.Base.AppAbs;
using ID.Email.Base.AppImps;
using ID.IntegrationEvents.Setup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ID.Email.Base.Setup;

public static class IdEmailBaseSetupExtensions
{

    //------------------------------------//

    /// <summary>
    /// Adds the ID Email Base services to the specified IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <returns>The IServiceCollection with the ID Email Base services added.</returns>
    internal static IServiceCollection AddIdEmailBase<TEmailService>(
        this IServiceCollection services, IdEmailBaseOptions emailOptions)
        where TEmailService : class, IIdEmailService
    {
        services.ConfigureEmailBaseOptions(emailOptions);

        services.RegisterServices<TEmailService>();
        return services;
    }

    //------------------------------------//

    /// <summary>
    /// Setup ID Email Base with configuration binding.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration instance to bind from.</param>
    /// <param name="sectionName">Optional section name. If null, assumes configuration is already the email section.</param>
    /// <returns>The same services</returns>
    internal static IServiceCollection AddIdEmailBase<TEmailService>(
        this IServiceCollection services, IConfiguration configuration, string? sectionName = null)
         where TEmailService : class, IIdEmailService
    {
        
        services.ConfigureEmailBaseOptions(configuration, sectionName);

        services.RegisterServices<TEmailService>();

        return services;
    }

    //------------------------------------//

    internal static IServiceCollection RegisterServices<TEmailService>(this IServiceCollection services) where TEmailService : class, IIdEmailService
    {
        services.TryAddSingleton<IEmailDetailsTemplateGenerator, EmailDetailsTemplateGenerator>();
        services.TryAddTransient<IIdEmailService, TEmailService>();
        services.TryAddSingleton<ITemplateHelpers, TemplateHelpers>();
        services.RegisterIdEventListeners(typeof(IdEmailBaseAssemblyReference).Assembly);
        return services;
    }


}//Cls

