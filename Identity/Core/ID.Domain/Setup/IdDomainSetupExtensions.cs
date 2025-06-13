using ID.Domain.AppServices.Abs;
using ID.Domain.AppServices.Imps;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ID.Domain.Setup;


public static class IdDomainSetupExtensions
{
    /// <summary>
    /// Setup IdDomain
    /// </summary>
    /// <returns>The same services</returns>
    public static IServiceCollection AddMyIdDomain(this IServiceCollection services)
    {
        //IdDomainSettings.Setup(setupOptions);

        services.TryAddScoped<ITeamBuilderService, TeamBuilderService>();
        return services;
    }


}//Cls

