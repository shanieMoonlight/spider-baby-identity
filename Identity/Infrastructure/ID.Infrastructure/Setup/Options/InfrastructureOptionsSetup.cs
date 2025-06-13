using ID.Domain.Utility.Messages;
using ID.Infrastructure.Setup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ID.Infrastructure.Setup.Options;

/// <summary>
/// Configureing the IdInfrastructureOptions in the IOptions Pattern.
/// </summary>
public static class IdInfrastructureOptionsSetup
{

    internal static IServiceCollection ConfigureInfrastructureOptions(this IServiceCollection services, IdInfrastructureSetupOptions setupOptions)
    {
        services.Configure<IdInfrastructureOptions>(infOptions =>
        {
            infOptions.UseDbTokenProvider = setupOptions.UseDbTokenProvider ?? InfrastructureDefaultValues.USE_DB_TOKEN_PROVIDER;
            infOptions.AllowExternalPagesDevModeAccess = setupOptions.AllowExternalPagesDevModeAccess ?? InfrastructureDefaultValues.ALLOW_EXTERNAL_PAGES_DEV_MODE_ACCESS;
            infOptions.SwaggerUrl = setupOptions.SwaggerUrl ?? InfrastructureDefaultValues.SWAGGER_URL;
            infOptions.ExternalPages = [.. setupOptions.ExternalPages ?? []];
        });

        return services;
    }


    //--------------------------//


    internal static IServiceCollection ConfigureInfrastructureOptions(
        this IServiceCollection services, IConfiguration configuration, string? sectionName = null)
    {
        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration), IDMsgs.Error.Setup.MISSING_CONFIGURATION);

        // If sectionName provided, get that section; otherwise assume configuration is already the JWT section
        var configSection = string.IsNullOrWhiteSpace(sectionName)
            ? configuration
            : configuration.GetSection(sectionName);

        services.Configure<IdInfrastructureOptions>(configSection);

        return services;
    }



}//Cls
