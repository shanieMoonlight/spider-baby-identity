using LogToFile.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;

namespace LogToFile.Setup;

public static class LogToFileSetupExtensions
{

    /// <summary>
    /// Set up Logger for use in app. 
    /// </summary>
    /// <param name="builder">Specifies the contract for a collection of service descriptors.</param>
    /// <param name="setupOptions">Specific setup details</param>
    /// <returns>IServiceCollection with LogToDatase attached</returns>
    public static ILoggingBuilder AddLogToFile(this ILoggingBuilder builder, LogToFileOptions setupOptions)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // 1. Add required dependencies first
        builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        // 2. Configure options
        builder.Services.ConfigureLogToFileOptions(setupOptions);

        // 3. Register the provider
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, FileLoggerProvider>());

        // 4. Register provider options
        LoggerProviderOptions.RegisterProviderOptions<LogToFileOptions, FileLoggerProvider>(builder.Services);

        return builder;
    }


    //-------------------------//


    /// <summary>
    /// Set up Logger for use in app. 
    /// </summary>
    /// <param name="services">Specifies the contract for a collection of service descriptors.</param>
    /// <param name="optionsConfig">Action describing how to set up LogToFileSetupOptions</param>
    /// <returns>IServiceCollection with LogToFile attached</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static ILoggingBuilder AddLogToFile(this ILoggingBuilder builder, Action<LogToFileOptions> optionsConfig)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var setupOptions = new LogToFileOptions();
        optionsConfig(setupOptions);

        builder.AddLogToFile(setupOptions);

        return builder;

    }



}//Cls


