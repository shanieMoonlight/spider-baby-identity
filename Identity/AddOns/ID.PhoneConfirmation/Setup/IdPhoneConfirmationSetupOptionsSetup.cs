using ID.Domain.Utility.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ID.PhoneConfirmation.Setup;
internal static class IdPhoneConfirmationSetupOptionsSetup
{

    internal static IServiceCollection ConfigureIdPhoneConfirmationSetupOptions(this IServiceCollection services, IdPhoneConfirmationSetupOptions setupOptions)
    {
        services.Configure<IdPhoneConfirmationSetupOptions>(IdPhoneConfirmationSetupOptions =>
        {
        });

        return services;
    }


    //-------------------------------------//

    internal static IServiceCollection ConfigureIdPhoneConfirmationSetupOptions(this IServiceCollection services, IConfiguration configuration, string? sectionName = null)
    {
        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration), IDMsgs.Error.Setup.MISSING_CONFIGURATION);

        // If sectionName provided, get that section; otherwise assume configuration is already the JWT section
        var configSection = string.IsNullOrWhiteSpace(sectionName)
            ? configuration
            : configuration.GetSection(sectionName);

        services.Configure<IdPhoneConfirmationSetupOptions>(configSection);

        return services;
    }



}//Cls
