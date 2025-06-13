using ID.IntegrationEvents.Setup;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ID.API.Setup.ExtraSetup;

public static class IdCustomEmailSetupExtensions
{

    /// <summary>
    /// Setup Custom Email
    /// </summary>
    /// <returns>The same services</returns>
    public static IServiceCollection AddMyIdCustomEmail<TAssemblyReference>(this IServiceCollection services) =>
        services.AddMyIdCustomEmail(typeof(TAssemblyReference).Assembly);

    //- - - - - - - - - - - - - - - - - - - - //

    /// <summary>
    /// Setup Custom Email
    /// </summary>
    /// <returns>The same services</returns>
    public static IServiceCollection AddMyIdCustomEmail(this IServiceCollection services, Assembly customEmailAssembly) => 
        services.RegisterIdEventListeners(customEmailAssembly);


}//Cls