using ID.Email.Base.Setup;
using ID.Email.SG.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ID.Email.SG.Setup;

public static class IdEmailSgSetupExtensions
{
    /// <summary>
    /// Setup MyId SendGrid Email from IConfiguration
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="sectionName"></param>
    /// <returns>The same services</returns>
    public static IServiceCollection AddMyIdEmailSG(this IServiceCollection services, IConfiguration configuration, string? sectionName = null)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        // If sectionName provided, get that section; otherwise assume configuration is already the JWT section
        var configSection = string.IsNullOrWhiteSpace(sectionName)
            ? configuration
            : configuration.GetSection(sectionName);

        services.AddOptionsWithValidateOnStart<IdEmailSgOptions, IdEmailSgOptionsValidator>()
            .Bind(configSection);

        // Add base email services configured from configuration
        services.AddIdEmailBase<IdEmailSgService>(configSection);

        return services;
    }


    //-----------------------//

    /// <summary>
    /// Setup MyId SendGrid Email using an action configuration
    /// </summary>
    /// <param name="services"></param>
    /// <param name="config"></param>
    /// <returns>The same services</returns>
    public static IServiceCollection AddMyIdEmailSG(this IServiceCollection services, Action<IdEmailSgOptions> config)
    {
        // Register validator to run on start
        services
            .AddOptionsWithValidateOnStart<IdEmailSgOptions, IdEmailSgOptionsValidator>()
            .Configure(config);

        // Create options instance and configure DI from values
        IdEmailSgOptions setupOptions = new();
        config(setupOptions);

        // Add base email services using concrete options
        services.AddIdEmailBase<IdEmailSgService>(setupOptions);

        return services;
    }

}//Cls

