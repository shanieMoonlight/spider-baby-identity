using Microsoft.Extensions.DependencyInjection;

namespace ClArch.ValueObjects.Setup;

public static class ValueObjectsSetupExtensions
{

    //------------------------------------//

    /// <summary>
    /// Setup IdDomain
    /// </summary>
    /// <param name="services"></param>
    /// <returns>The same services</returns>
    public static IServiceCollection AddValueObjects(this IServiceCollection services, ValueObjectsSetupOptions? setupOptions = null)
    {
        ValueObjectsSettings.Setup(setupOptions);

        return services;

    }

    //------------------------------------//

    /// <summary>
    /// Setup IdDomain
    /// </summary>
    /// <param name="services"></param>
    /// <returns>The same services</returns>
    public static IServiceCollection AddValueObjects(this IServiceCollection services, Action<ValueObjectsSetupOptions> config)
    {
        ValueObjectsSetupOptions setupOptions = new();
        config(setupOptions);

        return services.AddValueObjects(setupOptions);

    }

    //------------------------------------//

}//Cls

