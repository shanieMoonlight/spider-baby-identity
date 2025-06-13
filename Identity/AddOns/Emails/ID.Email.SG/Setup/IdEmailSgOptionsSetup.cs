using ID.Domain.Utility.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StringHelpers;

namespace ID.Email.SG.Setup;

internal static class IdEmailSgOptionsSetup
{
    /// <summary>
    /// Configures SendGrid email options with direct parameter values.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="sgOptions">The SendGrid options to configure.</param>
    /// <returns>The service collection for method chaining.</returns>
    internal static IServiceCollection ConfigureSendGridOptions(this IServiceCollection services, IdEmailSgOptions sgOptions)
    {
        ValidateSendGridOptions(sgOptions);

        // Configure SendGrid-specific options
        services.Configure<IdEmailSgOptions>(opts =>
        {
            opts.ApiKey = sgOptions.ApiKey;
        });

        return services;
    }

    //-------------------------------------//

    /// <summary>
    /// Configures SendGrid email options by binding from configuration.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">The configuration instance to bind from. Can be root config or a specific section.</param>
    /// <param name="sectionName">Optional section name. If null, assumes configuration is already the SendGrid section.</param>
    /// <returns>The service collection for method chaining.</returns>
    internal static IServiceCollection ConfigureSendGridOptions(
        this IServiceCollection services, IConfiguration configuration, string? sectionName = null)
    {
        if (configuration == null)
            throw new SetupDataException(nameof(IdEmailSgOptions), nameof(configuration));

        // If sectionName provided, get that section; otherwise assume configuration is already the JWT section
        var configSection = string.IsNullOrWhiteSpace(sectionName)
            ? configuration
            : configuration.GetSection(sectionName);

        services.Configure<IdEmailSgOptions>(configSection);


        return services;
    }

    //-------------------------------------//

    private static void ValidateSendGridOptions(IdEmailSgOptions options)
    {
        // Validate SendGrid-specific options
        if (options.ApiKey.IsNullOrWhiteSpace())
            throw new SetupDataException(nameof(IdEmailSgOptionsSetup), nameof(options.ApiKey));
    }

}
