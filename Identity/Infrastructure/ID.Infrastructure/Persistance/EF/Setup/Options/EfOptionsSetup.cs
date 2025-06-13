using ID.Domain.Utility.Messages;
using ID.Infrastructure.Setup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StringHelpers;


namespace ID.Infrastructure.Persistance.EF.Setup.Options;


/// <summary>
/// Configureing the IdInfrastructureOptions in the IOptions Pattern.
/// </summary>
public static class IdInfrastructureOptionsSetup
{

    internal static IServiceCollection ConfigureEfOptions(this IServiceCollection services, IdInfrastructureSetupOptions setupOptions)
    {
        
        if (setupOptions.ConnectionString.IsNullOrWhiteSpace())
            throw new ArgumentNullException(nameof(setupOptions), IDMsgs.Error.Setup.MISSING_CONNECTION_STRING);


        services.Configure<EfOptions>(infOptions =>
        {
            infOptions.ConnectionString = setupOptions.ConnectionString!;
        });

        return services;
    }


    //--------------------------//


    internal static IServiceCollection ConfigureEfOptions(
        this IServiceCollection services, IConfiguration configuration, string? sectionName = null)
    {
        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration), IDMsgs.Error.Setup.MISSING_CONFIGURATION);

        // If sectionName provided, get that section; otherwise assume configuration is already the JWT section
        var configSection = string.IsNullOrWhiteSpace(sectionName)
            ? configuration
            : configuration.GetSection(sectionName);

        services.Configure<EfOptions>(configSection);

        return services;
    }



}//Cls
