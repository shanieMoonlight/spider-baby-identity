using ID.Domain.Utility.Messages;
using ID.Infrastructure.Setup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ID.Infrastructure.Setup.SignIn;

/// <summary>
/// Configuring the IdSignInOptions in the IOptions Pattern.
/// </summary>
public static class IdSignInOptionsSetup
{

    internal static IServiceCollection ConfigureSignInOptions(this IServiceCollection services, IdInfrastructureSetupOptions setupOptions)
    {
        services.Configure<IdSignInOptions>(signInOptions =>
        {
            signInOptions.RequireConfirmedEmail = setupOptions.SignInOptions?.RequireConfirmedEmail ?? true;
            signInOptions.RequireConfirmedPhoneNumber = setupOptions.SignInOptions?.RequireConfirmedPhoneNumber ?? false;
        });

        return services;
    }


    //-------------------------------------//

    internal static IServiceCollection ConfigureSignInOptions(this IServiceCollection services, IConfiguration configuration, string? sectionName = null)
    {
        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration), IDMsgs.Error.Setup.MISSING_CONFIGURATION);

        // If sectionName provided, get that section; otherwise assume configuration is already the SignIn section
        var configSection = string.IsNullOrWhiteSpace(sectionName)
            ? configuration
            : configuration.GetSection(sectionName);

        services.Configure<IdSignInOptions>(configSection);

        return services;
    }


}//Cls
