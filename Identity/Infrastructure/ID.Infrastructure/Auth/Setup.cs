using ID.Infrastructure.Auth.AppAbs;
using ID.Infrastructure.Auth.AppImps;
using ID.Infrastructure.Setup;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ID.Infrastructure.Auth;
internal static class AuthSetup
{

    public static IServiceCollection SetupAuthChallengers(this IServiceCollection services, IdInfrastructureSetupOptions options)//, IConfiguration iOptionsConfig
    {

        services.TryAddScoped<IExternalPageAuthenticationService, ExternalPageAuthenticationService>();
        services.TryAddScoped<IExternalPageListService, ExternalPageListService>();

        return services;
    }


}
