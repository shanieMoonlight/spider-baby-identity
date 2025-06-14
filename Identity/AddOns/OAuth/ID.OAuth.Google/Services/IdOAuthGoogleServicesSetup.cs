using ID.Domain.Entities.AppUsers;
using ID.OAuth.Google.Services.Abs;
using ID.OAuth.Google.Services.Imps;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ID.OAuth.Google.Setup;
internal static class IdOAuthGoogleServicesSetup
{
    internal static IServiceCollection RegisterServices<TUser>(this IServiceCollection services) where TUser : AppUser
    {
        services.TryAddScoped<IGoogleTokenVerifier, GoogleTokenVerifier>();
        services.TryAddScoped<IFindOrCreateService<TUser>, FindOrCreateService<TUser>>();

        return services;
    }




}//Cls
