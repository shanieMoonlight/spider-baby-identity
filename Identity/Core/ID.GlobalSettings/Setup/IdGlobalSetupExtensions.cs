using ID.GlobalSettings.Setup.Options;
using Microsoft.Extensions.DependencyInjection;

namespace ID.GlobalSettings.Setup;

internal static class IdGlobalSetupExtensions
{
     
    /// <summary>
    /// Setup Global Settings for the ID library
    /// </summary>
    /// <param name="services"></param>
    /// <returns>The same services</returns>
    internal static IServiceCollection ConfigureGlobalSettings(
        this IServiceCollection services, IdGlobalOptions setupOptions)
    {
        // Use the new setup method for consistency and validation
        services.ConfigureGlobalOptions(setupOptions);

        return services;
    }


    //-----------------------------//    
    
    
    /// <summary>
    /// Setup Global Settings for CUSTOMER functionality in the ID library
    /// </summary>
    /// <param name="services"></param>
    /// <returns>The same services</returns>
    internal static IServiceCollection Configure_CUSTOMER_GlobalSettings(
        this IServiceCollection services, IdGlobalSetupOptions_CUSTOMER setupOptions)
    {
        // Use the new setup method for consistency and validation
        services.ConfigureCustomerGlobalOptions(setupOptions);


        return services;
    }


}//Cls

