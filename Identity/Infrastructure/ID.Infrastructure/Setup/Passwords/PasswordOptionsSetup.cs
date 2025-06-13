using ID.Domain.Utility.Messages;
using ID.Infrastructure.Setup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ID.Infrastructure.Setup.Passwords;

/// <summary>
/// Configureing the IdPasswordOptions in the IOptions Pattern.
/// </summary>
public static class IdPasswordOptionsSetup
{

    internal static IServiceCollection ConfigurePasswordOptions(this IServiceCollection services, IdInfrastructureSetupOptions setupOptions)
    {
        services.Configure<IdPasswordOptions>(pwdOptions =>
        {
            pwdOptions.RequiredLength = setupOptions.PasswordOptions?.RequiredLength ?? IdPasswordDefaultValues.RequiredLength;
            pwdOptions.RequiredUniqueChars = setupOptions.PasswordOptions?.RequiredUniqueChars ?? IdPasswordDefaultValues.RequiredUniqueChars;
            pwdOptions.RequireNonAlphanumeric = setupOptions.PasswordOptions?.RequireNonAlphanumeric ?? IdPasswordDefaultValues.RequireNonAlphanumeric;
            pwdOptions.RequireLowercase = setupOptions.PasswordOptions?.RequireLowercase ?? IdPasswordDefaultValues.RequireLowercase;
            pwdOptions.RequireUppercase = setupOptions.PasswordOptions?.RequireUppercase ?? IdPasswordDefaultValues.RequireUppercase;
            pwdOptions.RequireDigit = setupOptions.PasswordOptions?.RequireDigit ?? IdPasswordDefaultValues.RequireDigit;
        });

        return services;
    }


    //-------------------------------------//

    internal static IServiceCollection ConfigurePasswordOptions(
        this IServiceCollection services, IConfiguration configuration, string? sectionName = null)
    {
        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration), IDMsgs.Error.Setup.MISSING_CONFIGURATION);

        // If sectionName provided, get that section; otherwise assume configuration is already the JWT section
        var configSection = string.IsNullOrWhiteSpace(sectionName)
            ? configuration
            : configuration.GetSection(sectionName);

        services.Configure<IdPasswordOptions>(configSection);

        return services;
    }



}//Cls
