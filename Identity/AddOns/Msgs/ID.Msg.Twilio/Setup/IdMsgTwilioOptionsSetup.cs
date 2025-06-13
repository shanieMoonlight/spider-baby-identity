using StringHelpers;
using ID.Domain.Utility.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ID.Msg.Twilio.Setup;

internal static class IdMsgTwilioOptionsSetup
{    /// <summary>
     /// Configures Twilio options by directly setting values from an options object.
     /// </summary>
     /// <param name="services">The service collection to configure.</param>
     /// <param name="twilioOptions">The Twilio options to configure.</param>
     /// <returns>The service collection for method chaining.</returns>
    internal static IServiceCollection ConfigureTwilioOptions(this IServiceCollection services,
        IdMsgTwilioOptions twilioOptions)
    {
        ValidateTwilioOptions(twilioOptions);

        services.Configure<IdMsgTwilioOptions>(opts => CopyOptionsValues(twilioOptions, opts));

        return services;
    }

    //-------------------------------------//

    /// <summary>
    /// Configures Twilio options by binding from configuration.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">The configuration instance to bind from. Can be root config or a specific section.</param>
    /// <param name="sectionName">Optional section name. If null, assumes configuration is already the Twilio section.</param>
    /// <returns>The service collection for method chaining.</returns>
    internal static IServiceCollection ConfigureTwilioOptions(this IServiceCollection services,
        IConfiguration configuration,
        string? sectionName = null)
    {
        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration), "Configuration cannot be null when setting up Twilio options.");

        // If sectionName provided, get that section; otherwise assume configuration is already the Twilio section
        var configSection = string.IsNullOrWhiteSpace(sectionName)
            ? configuration
            : configuration.GetSection(sectionName);

        services.Configure<IdMsgTwilioOptions>(configSection);

        return services;
    }

    //-------------------------------------//

    private static void CopyOptionsValues(IdMsgTwilioOptions source, IdMsgTwilioOptions target)
    {
        target.TwilioId = source.TwilioId;
        target.TwilioPassword = source.TwilioPassword;
        target.TwilioFromNumber = source.TwilioFromNumber;
    }

    //-------------------------------------//

    private static void ValidateTwilioOptions(IdMsgTwilioOptions twilioOptions)
    {
        if (twilioOptions == null)
            throw new SetupDataException( nameof(IdMsgTwilioOptionsSetup), nameof(twilioOptions));

        if (twilioOptions.TwilioId.IsNullOrWhiteSpace())
            throw new SetupDataException(nameof(IdMsgTwilioOptionsSetup), nameof(twilioOptions.TwilioId));

        if (twilioOptions.TwilioPassword.IsNullOrWhiteSpace())
            throw new SetupDataException(nameof(IdMsgTwilioOptionsSetup), nameof(twilioOptions.TwilioPassword));

        if (twilioOptions.TwilioFromNumber.IsNullOrWhiteSpace())
            throw new SetupDataException(nameof(IdMsgTwilioOptionsSetup), nameof(twilioOptions.TwilioFromNumber));
    }
}
