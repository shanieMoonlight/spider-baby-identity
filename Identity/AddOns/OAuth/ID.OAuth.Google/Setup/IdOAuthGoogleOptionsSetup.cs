using ID.Domain.Utility.Exceptions;
using ID.Domain.Utility.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StringHelpers;

namespace ID.OAuth.Google.Setup;
internal static class IdOAuthGoogleOptionsSetup
{

    internal static IServiceCollection ConfigureIdOAuthGoogleOptions(this IServiceCollection services, IdOAuthGoogleOptions setupOptions)
    {
        ValidateOAuthGoogleOptions(setupOptions);

        services.Configure<IdOAuthGoogleOptions>(opts =>
        {
            opts.ClientSecret = setupOptions.ClientSecret!;
            opts.ClientId = setupOptions.ClientId!;
        });



        return services;
    }


    //-------------------------------------//

    internal static IServiceCollection ConfigureIdOAuthGoogleOptions(this IServiceCollection services, IConfiguration configuration, string? sectionName = null)
    {
        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration), IDMsgs.Error.Setup.MISSING_CONFIGURATION);

        // If sectionName provided, get that section; otherwise assume configuration is already the JWT section
        var configSection = string.IsNullOrWhiteSpace(sectionName)
            ? configuration
            : configuration.GetSection(sectionName);

        services.Configure<IdOAuthGoogleOptions>(configSection);

        return services;
    }

    //-------------------------------------//

    private static void ValidateOAuthGoogleOptions(IdOAuthGoogleOptions options)
    {
        // Validate SendGrid-specific options
        //if (options.ClientSecret.IsNullOrWhiteSpace())
        //    throw new SetupDataException(nameof(IdOAuthGoogleOptions), nameof(options.ClientSecret));


        if (options.ClientId.IsNullOrWhiteSpace())
            throw new SetupDataException(nameof(IdOAuthGoogleOptions), nameof(options.ClientId));
    }



}//Cls
