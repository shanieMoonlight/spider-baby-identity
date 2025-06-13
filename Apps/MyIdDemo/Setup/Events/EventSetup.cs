using Microsoft.Extensions.DependencyInjection;
using MyIdDemo.Setup.Events.MT;
using System.Reflection;

namespace MyIdDemo.Setup.Events;

/// <summary>
///  Setup Event pub/sub
/// </summary>
public static class EventSetup
{

    public static IServiceCollection AddMyIdDemoEvents(this IServiceCollection services)
    {
        services.AddEventsMT(MyIdDemoAssemblyReference.Assembly);
        return services;
    }

    //------------------------------------//

    public static IServiceCollection RegisterMyIdDemoEventListeners(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.RegisterListenersMT(assemblies);
        return services;
    }


}
