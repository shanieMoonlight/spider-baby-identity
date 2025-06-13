using ID.Email.SMTP.Utility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ID.Email.SMTP.Setup;

/// <summary>
/// Setup class for SMTP email options.
/// </summary>
internal static class IdEmailSmtpOptionsSetup
{

    /// <summary>
    /// Configures SMTP options from direct values and validates them.
    /// </summary>
    internal static IServiceCollection ConfigureEmailSmtpOptions(
        this IServiceCollection services, IdEmailSmtpOptions setupOptions)
    {
        ValidateOptions(setupOptions);

        // Configure SMTP-specific options
        services.Configure<IdEmailSmtpOptions>(opts =>
        {
            opts.SmtpServerAddress = setupOptions.SmtpServerAddress;
            opts.SmtpPortNumber = setupOptions.SmtpPortNumber;
            opts.SmtpUsernameOrEmail = setupOptions.SmtpUsernameOrEmail;
            opts.SmtpPassword = setupOptions.SmtpPassword;
        });

        return services;
    }


    //------------------------------------//


    /// <summary>
    /// Configures SMTP options from IConfiguration and validates them.
    /// </summary>
    internal static IServiceCollection ConfigureEmailSmtpOptionsFromConfiguration(
        this IServiceCollection services, IConfiguration configuration, string? sectionName = null)
    {
        if (configuration == null)
            throw new IdEmailSmtpMissingSetupException(nameof(configuration));

        // If sectionName provided, get that section; otherwise assume configuration is already the JWT section
        var configSection = string.IsNullOrWhiteSpace(sectionName)
            ? configuration
            : configuration.GetSection(sectionName);

        services.Configure<IdEmailSmtpOptions>(configSection);


        return services;
    }


    //------------------------------------//


    /// <summary>
    /// Validates the SMTP options.
    /// </summary>
    public static void ValidateOptions(IdEmailSmtpOptions options)
    {
        // Validate SMTP-specific options
        if (string.IsNullOrWhiteSpace(options.SmtpServerAddress))
            throw new IdEmailSmtpMissingSetupException(nameof(options.SmtpServerAddress));

        if (options.SmtpPortNumber <= 0)
            throw new IdEmailSmtpInvalidSetupException("SMTP port number must be greater than 0.");

        if (string.IsNullOrWhiteSpace(options.SmtpUsernameOrEmail))
            throw new IdEmailSmtpMissingSetupException(nameof(options.SmtpUsernameOrEmail));

        if (string.IsNullOrWhiteSpace(options.SmtpPassword))
            throw new IdEmailSmtpMissingSetupException(nameof(options.SmtpPassword));
    }


}//Cls
