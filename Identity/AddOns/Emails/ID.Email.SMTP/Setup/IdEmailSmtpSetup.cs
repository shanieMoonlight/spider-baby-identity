using ID.Email.Base.Setup;
using ID.Email.SMTP.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ID.Email.SMTP.Setup;

/// <summary>
/// Setup class for SMTP email options.
/// </summary>
public static class IdEmailSmtpSetup
{

    /// <summary>
    /// Configures SMTP options from direct values and validates them.
    /// </summary>
    public static IServiceCollection AddMyIdEmailSmtp(
        this IServiceCollection services, Action<IdEmailSmtpOptions> config)
    {
        // Register options with validator to run on start
        services
            .AddOptionsWithValidateOnStart<IdEmailSmtpOptions, IdEmailSmtpOptionsValidator>()
            .Configure(config);

        // Create options instance and configure DI from values
        IdEmailSmtpOptions setupOptions = new();
        config(setupOptions);
        services.AddIdEmailBase<IdEmailSmtpService>(setupOptions);

        return services;
    }


    //-----------------------//


    /// <summary>
    /// Configures SMTP options from IConfiguration and validates them.
    /// </summary>
    public static IServiceCollection AddMyIdEmailSmtp(
        this IServiceCollection services, IConfiguration configuration, string? sectionName = null)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        // If sectionName provided, get that section; otherwise assume configuration is already the JWT section
        var configSection = string.IsNullOrWhiteSpace(sectionName)
            ? configuration
            : configuration.GetSection(sectionName);

        // Register options binding and validate on start using validator
        services
            .AddOptionsWithValidateOnStart<IdEmailSmtpOptions, IdEmailSmtpOptionsValidator>()
            .Bind(configSection);

        // Add base email services configured from configuration
        services.AddIdEmailBase<IdEmailSmtpService>(configSection);

        return services;
    }

}//Cls
