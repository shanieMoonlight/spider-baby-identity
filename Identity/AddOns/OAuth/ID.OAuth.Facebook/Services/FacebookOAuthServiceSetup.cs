using ID.Domain.Entities.AppUsers;
using ID.OAuth.Facebook.HttpService.Abs;
using ID.OAuth.Facebook.HttpService.Imps;
using ID.OAuth.Facebook.Services.Abs;
using ID.OAuth.Facebook.Services.Imps;
using Microsoft.Extensions.DependencyInjection;

namespace ID.OAuth.Facebook.Services;

/// <summary>
/// Extension methods for configuring Facebook OAuth services.
/// </summary>
public static class FacebookOAuthServiceSetup
{


    public static IServiceCollection AddFacebookOAuthServices(this IServiceCollection services)
    {


        // Register Facebook OAuth services
        services.AddScoped<IFacebookAuthenticationService, FacebookAuthenticationService>();
        services.AddScoped<IFindOrCreateService<AppUser>, FindOrCreateService<AppUser>>();

        return services;
    }


}//Cls
