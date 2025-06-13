using ID.GlobalSettings.Exceptions;
using ID.GlobalSettings.Setup.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StringHelpers;



namespace ID.GlobalSettings.Setup;

internal static class IdGlobalOptionsSetup
{
    /// <summary>
    /// Configures global settings options with direct parameter values.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="setupOptions">The global options to configure.</param>
    /// <returns>The service collection for method chaining.</returns>
    internal static IServiceCollection ConfigureGlobalOptions(this IServiceCollection services, IdGlobalOptions setupOptions)
    {
        ValidateGlobalOptions(setupOptions);

        services.Configure<IdGlobalOptions>(opts =>
        {
            opts.ApplicationName = setupOptions.ApplicationName;
            opts.MntcAccountsUrl = setupOptions.MntcAccountsUrl;
            opts.MntcTeamMaxPosition = setupOptions.MntcTeamMaxPosition;
            opts.MntcTeamMinPosition = setupOptions.MntcTeamMinPosition;
            opts.SuperTeamMaxPosition = setupOptions.SuperTeamMaxPosition;
            opts.SuperTeamMinPosition = setupOptions.SuperTeamMinPosition;
            opts.ClaimTypePrefix = setupOptions.ClaimTypePrefix;
            opts.JwtRefreshTokensEnabled = setupOptions.JwtRefreshTokensEnabled;
            opts.PhoneTokenTimeSpan = setupOptions.PhoneTokenTimeSpan;
        });

        return services;
    }


    //-------------------------------------//


    /// <summary>
    /// Configures IdGlobal options by binding from configuration.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">The configuration instance to bind from. Can be root config or a specific section.</param>
    /// <param name="sectionName">Optional section name. If null, assumes configuration is already the global section.</param>
    /// <returns>The service collection for method chaining.</returns>
    internal static IServiceCollection ConfigureGlobalOptions(this IServiceCollection services,
        IConfiguration configuration,
        string? sectionName = null)
    {
        if (configuration == null)
            throw new GlobalSettingMissingSetupDataException(nameof(configuration));

        // If sectionName provided, get that section; otherwise assume configuration is already the global section
        var configSection = string.IsNullOrWhiteSpace(sectionName)
            ? configuration
            : configuration.GetSection(sectionName);

        services.Configure<IdGlobalOptions>(configSection);

        return services;
    }


    //-----------------------------//


    private static void ValidateGlobalOptions(IdGlobalOptions options)
    {
        if (options.ApplicationName.IsNullOrWhiteSpace())
            throw new GlobalSettingMissingSetupDataException(nameof(options.ApplicationName));


        if (options.MntcAccountsUrl.IsNullOrWhiteSpace())
            throw new GlobalSettingMissingSetupDataException(nameof(options.MntcAccountsUrl));


        if (options.MntcTeamMaxPosition < 1)
            throw new GlobalSettingInvalidSetupDataException(
                nameof(options.MntcTeamMaxPosition), $"{nameof(options.MntcTeamMaxPosition)} must be greater than 0.");


        if (options.MntcTeamMinPosition < 1)
            throw new GlobalSettingInvalidSetupDataException(
                nameof(options.MntcTeamMinPosition), $"{nameof(options.MntcTeamMinPosition)} must be greater than 0.");


        if (options.MntcTeamMinPosition > options.MntcTeamMaxPosition)
            throw new GlobalSettingInvalidSetupDataException(
                nameof(options.MntcTeamMinPosition), $"{nameof(options.MntcTeamMinPosition)} must not be greater than {nameof(options.MntcTeamMaxPosition)}.");




        if (options.SuperTeamMaxPosition < 1)
            throw new GlobalSettingInvalidSetupDataException(
                nameof(options.SuperTeamMaxPosition), $"{nameof(options.SuperTeamMaxPosition)} must be greater than 0.");


        if (options.SuperTeamMinPosition < 1)
            throw new GlobalSettingInvalidSetupDataException(
                nameof(options.SuperTeamMinPosition), $"{nameof(options.SuperTeamMinPosition)} must be greater than 0.");


        if (options.SuperTeamMinPosition > options.SuperTeamMaxPosition)
            throw new GlobalSettingInvalidSetupDataException(
                nameof(options.SuperTeamMinPosition), $"{nameof(options.SuperTeamMinPosition)} must not be greater than {nameof(options.SuperTeamMaxPosition)}.");


        if (options.PhoneTokenTimeSpan <= TimeSpan.Zero)
            throw new GlobalSettingInvalidSetupDataException(
                nameof(options.PhoneTokenTimeSpan), $"{nameof(options.PhoneTokenTimeSpan)} must be greater than TimeSpan.Zero.");
    }

}//Cls
