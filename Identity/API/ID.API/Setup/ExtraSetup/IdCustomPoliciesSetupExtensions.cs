using CollectionHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace ID.API.Setup.ExtraSetup;

public static class IdCustomPoliciesSetupExtensions
{

    /// <summary>
    /// Add your own custom policies to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection
    /// </summary>
    /// <typeparam name="Handler">THe type of auth handler that will deal with the added policy</typeparam>
    /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection to add services to.</param>
    /// <param name="policyName">The name of the policy to add</param>
    /// <param name="requirements">The requirements that the policy will have</param>
    /// <returns>The Microsoft.Extensions.DependencyInjection.IServiceCollection so that additional calls can be chained.</returns>
    public static IServiceCollection AddPolicy<Handler>(
        this IServiceCollection services, string policyName, IEnumerable<IAuthorizationRequirement> requirements) where Handler : class, IAuthorizationHandler
    {
        services.AddScoped<IAuthorizationHandler, Handler>();
     
        return services.AddAuthorization(options => 
            options.AddPolicy(policyName, policy =>
                  policy
                    .RequireAuthenticatedUser()
                    .Requirements.AddRange(requirements))
        );

    }

}//Cls