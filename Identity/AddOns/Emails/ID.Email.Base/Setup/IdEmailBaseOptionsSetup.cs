using ID.Domain.Utility.Exceptions;
using ID.GlobalSettings.Setup.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StringHelpers;

namespace ID.Email.Base.Setup;

internal static class IdEmailBaseOptionsSetup
{
    /// <summary>
    /// Configures email base options with direct parameter values.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="emailOptions">The email options to configure.</param>
    /// <returns>The service collection for method chaining.</returns>    

    internal static IServiceCollection ConfigureEmailBaseOptions(
        this IServiceCollection services, IdEmailBaseOptions emailOptions)
    {
        // Validate the input options immediately
        ValidateEmailBaseOptions(emailOptions);

        services.AddSingleton<IConfigureOptions<IdEmailBaseOptions>>(serviceProvider =>
        {
            return new ConfigureNamedOptions<IdEmailBaseOptions>(null, options =>
            {
                var globalOptions = serviceProvider.GetRequiredService<IOptions<IdGlobalOptions>>().Value;

                // Configure all properties in one place
                options.LogoUrl = emailOptions.LogoUrl;
                options.FromAddress = emailOptions.FromAddress;
                options.ToAddresses = emailOptions.ToAddresses;
                options.CcAddresses = emailOptions.CcAddresses;
                options.BccAddresses = emailOptions.BccAddresses;

                options.ColorHexBrand = emailOptions.ColorHexBrand.IsNullOrWhiteSpace()
                    ? MyIdEmailDefaultValues.COLOR_HEX_BRAND
                    : emailOptions.ColorHexBrand;

                options.FromName = emailOptions.FromName.IsNullOrWhiteSpace()
                    ? globalOptions.ApplicationName
                    : emailOptions.FromName;
            });
        });

        return services;
    }

    //-------------------------------------//

    /// <summary>
    /// Configures email base options by binding from configuration.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">The configuration instance to bind from. Can be root config or a specific section.</param>
    /// <param name="sectionName">Optional section name. If null, assumes configuration is already the email section.</param>
    /// <returns>The service collection for method chaining.</returns>
    internal static IServiceCollection ConfigureEmailBaseOptions(this IServiceCollection services,
        IConfiguration configuration,
        string? sectionName = null)
    {
        if (configuration == null)
            throw new SetupDataException(nameof(configuration), nameof(IdEmailBaseOptions));

        // If sectionName provided, get that section; otherwise assume configuration is already the email section
        var configSection = string.IsNullOrWhiteSpace(sectionName)
            ? configuration
            : configuration.GetSection(sectionName);

        services.Configure<IdEmailBaseOptions>(configSection);

        return services;
    }

    //-------------------------------------//

    private static void ValidateEmailBaseOptions(IdEmailBaseOptions options)
    {
        if (options.FromAddress.IsNullOrWhiteSpace())
            throw new SetupDataException(nameof(options.FromAddress), nameof(IdEmailBaseOptions));
    }


}//Cls
