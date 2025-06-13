using ID.GlobalSettings.Exceptions;
using ID.GlobalSettings.Setup.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StringHelpers;

namespace ID.GlobalSettings.Setup;

internal static class IdGlobalCustomerOptionsSetup
{
    /// <summary>
    /// Configures global settings options with direct parameter values.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="setupOptions">The global options to configure.</param>
    /// <returns>The service collection for method chaining.</returns>
    internal static IServiceCollection ConfigureCustomerGlobalOptions(this IServiceCollection services, IdGlobalSetupOptions_CUSTOMER setupOptions)
    {
        ValidateGlobalOptions(setupOptions);


        services.Configure<IdGlobalSetupOptions_CUSTOMER>(opts =>
        {
            opts.CustomerAccountsUrl = setupOptions.CustomerAccountsUrl;
            opts.MaxTeamPosition = setupOptions.MaxTeamPosition;
            opts.MinTeamPosition = setupOptions.MinTeamPosition;
            opts.MaxTeamSize = setupOptions.MaxTeamSize;
        });

        return services;
    }

    //-------------------------------------//


    /// <summary>
    /// Configures IdGlobal_Customer options by binding from configuration.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">The configuration instance to bind from. Can be root config or a specific section.</param>
    /// <param name="sectionName">Optional section name. If null, assumes configuration is already the global section.</param>
    /// <returns>The service collection for method chaining.</returns>
    internal static IServiceCollection ConfigureCustomerGlobalOptions(this IServiceCollection services,
        IConfiguration configuration,
        string? sectionName = null)
    {
        if (configuration == null)
            throw new GlobalSettingMissingSetupDataException(nameof(configuration));

        // If sectionName provided, get that section; otherwise assume configuration is already the global section
        var configSection = string.IsNullOrWhiteSpace(sectionName)
            ? configuration
            : configuration.GetSection(sectionName);

        services.Configure<IdGlobalSetupOptions_CUSTOMER>(configSection);

        return services;
    }


    //-----------------------------//

    private static void ValidateGlobalOptions(IdGlobalSetupOptions_CUSTOMER options)
    {
        // Only validate required fields that can't be auto-corrected
        if (options.CustomerAccountsUrl.IsNullOrWhiteSpace())
            throw new GlobalSettingMissingSetupDataException(nameof(options.CustomerAccountsUrl));

        // Note: Numeric validations removed since setters auto-correct invalid values to defaults
        // The Min/Max position relationship is validated after setter corrections are applied
        if (options.MinTeamPosition > options.MaxTeamPosition)
            throw new GlobalSettingInvalidSetupDataException(
                nameof(options.MinTeamPosition), $"{nameof(options.MinTeamPosition)} must not be greater than {nameof(options.MaxTeamPosition)}.");
    }

}//Cls