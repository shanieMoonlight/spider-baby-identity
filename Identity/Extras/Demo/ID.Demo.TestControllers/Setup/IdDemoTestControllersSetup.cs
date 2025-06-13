using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;

namespace ID.Demo.TestControllers.Setup;

/// <summary>
///  Setup Event pub/sub
/// </summary>
public static class IdDemoTestControllersSetup
{
    public static IServiceCollection AddMyIdDemoTestControllers(this IServiceCollection services)
    {
        services.AddControllers()
            .PartManager.ApplicationParts.Add(new AssemblyPart(typeof(IdDemoTestControllersAssemblyReference).Assembly));
        return services;
    }


}//Cls
