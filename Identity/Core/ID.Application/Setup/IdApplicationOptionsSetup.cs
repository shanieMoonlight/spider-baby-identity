using ID.Domain.Utility.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ID.Application.Setup;
internal static class IdApplicationOptionsSetup
{
    /// <summary>
    /// Configures IdApplication options with direct parameter values.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="setupOptions">The email options to configure.</param>
    /// <returns>The service collection for method chaining.</returns>
    internal static IServiceCollection ConfigureApplicationOptions(this IServiceCollection services, IdApplicationOptions setupOptions)
    {
        ValidateApplicationOptions(setupOptions);

        services.Configure<IdApplicationOptions>(opts =>
        {
            opts.FromAppHeaderKey = setupOptions.FromAppHeaderKey;
            opts.FromAppHeaderValue = setupOptions.FromAppHeaderValue;
        });

        return services;
    }


    //-----------------------------//


    /// <summary>
    /// Configures IdApplication options by binding from configuration.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">The configuration instance to bind from. Can be root config or a specific section.</param>
    /// <param name="sectionName">Optional section name. If null, assumes configuration is already the email section.</param>
    /// <returns>The service collection for method chaining.</returns>
    internal static IServiceCollection ConfigureApplicationOptions(this IServiceCollection services,
        IConfiguration configuration,
        string? sectionName = null)
    {
        if (configuration == null)
            throw new SetupDataException(nameof(configuration), nameof(IdApplicationOptions));

        // If sectionName provided, get that section; otherwise assume configuration is already the application section
        var configSection = string.IsNullOrWhiteSpace(sectionName)
            ? configuration
            : configuration.GetSection(sectionName);

        services.Configure<IdApplicationOptions>(configSection);

        return services;
    }

    //-----------------------------//

    private static void ValidateApplicationOptions(IdApplicationOptions options)
    {
    }


}//Cls