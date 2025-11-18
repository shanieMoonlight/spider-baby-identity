using ID.Domain.Entities.AppUsers;
using ID.OAuth.Amazon.Services.Abs;
using ID.OAuth.Amazon.Services.Imps;
using Microsoft.Extensions.DependencyInjection;

namespace ID.OAuth.Amazon.Services;

/// <summary>
/// Extension methods for configuring Amazon OAuth services.
/// </summary>
public static class AmazonOAuthServicesExtensions
{


    public static IServiceCollection AddAmazonOAuthServices(this IServiceCollection services)
    {


        // Register Amazon OAuth services
        services.AddScoped<IAmazonAuthenticationService, AmazonAuthenticationService>();

        //TODO: Implement FindOrCreateService for Amazon OAuth  or move to Shared Utils????
        //services.AddScoped<IFindOrCreateService<AppUser>, FindOrCreateService<AppUser>>();

        return services;
    }


}//Cls
