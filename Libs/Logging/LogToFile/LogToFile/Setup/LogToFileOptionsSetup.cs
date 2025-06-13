using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StringHelpers;

namespace LogToFile.Setup;

public static class LogToFileOptionsSetup
{
    /// <summary>
    /// Configures LogToFile options with direct parameter values.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="options">The LogToFile options to configure.</param>
    /// <returns>The service collection for method chaining.</returns>
    internal static IServiceCollection ConfigureLogToFileOptions(this IServiceCollection services, LogToFileOptions options)
    {
        ValidateLogToFileOptions(options);

        // Configure LogToFile-specific options
        services.Configure<LogToFileOptions>(opts =>
        {
            opts.AppName = options.AppName;

            opts.FileDirectory = options.FileDirectory.IsNullOrWhiteSpace()
            ? DefaultValues.FILE_DIRECTORY
            : options.FileDirectory;

            opts.FileExtension = options.FileExtension.IsNullOrWhiteSpace()
            ? DefaultValues.FILE_EXTENSION
            : options.FileExtension;

            opts.FileName = options.FileName.IsNullOrWhiteSpace()
            ? $"{options.AppName}_{DefaultValues.FILE_NAME}"
            : options.FileName;

            opts.MaxMessageLength = options.MaxMessageLength > 0
            ? DefaultValues.MAX_MESSAGE_LENGTH
            : options.MaxMessageLength;

            opts.MaxMessageLength = options.MaxMessageLength > 0
            ? DefaultValues.MAX_MESSAGE_LENGTH
            : options.MaxMessageLength;

            opts.AsyncTimeoutSeconds = options.AsyncTimeoutSeconds > 0
            ? DefaultValues.ASYNC_TIMEOUT_SECONDS
            : options.AsyncTimeoutSeconds;


        });

        return services;
    }


    //-------------------------//


    /// <summary>
    /// Configures LogToFile options by binding from configuration.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">The configuration instance to bind from. Can be root config or a specific section.</param>
    /// <param name="sectionName">Optional section name. If null, assumes configuration is already the LogToFile section.</param>
    /// <returns>The service collection for method chaining.</returns>
    internal static IServiceCollection ConfigureLogToFileOptions(
        this IServiceCollection services, IConfiguration configuration, string? sectionName = null)
    {
        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration), "Configuration is required");

        // If sectionName provided, get that section; otherwise assume configuration is already the JWT section
        var configSection = string.IsNullOrWhiteSpace(sectionName)
            ? configuration
            : configuration.GetSection(sectionName);

        services.Configure<LogToFileOptions>(configSection);


        return services;
    }


    //-------------------------//


    private static void ValidateLogToFileOptions(LogToFileOptions options)
    {
        // Validate LogToFile-specific options

        if (options == null)
            throw new ArgumentNullException(nameof(options), $"{nameof(LogToFileOptions)}: You must at least supply the AppName.");

        if (string.IsNullOrWhiteSpace(options.AppName))
            throw new ArgumentNullException(nameof(options), $"{nameof(LogToFileOptions)}: You must supply an Application name when using LogToFile");
    }

    
}//Cls

