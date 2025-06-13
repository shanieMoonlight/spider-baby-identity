using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace ID.Infrastructure.Setup;

public static class IdInfrastructure_Extra_SetupExtensions
{
    public static IServiceCollection AuthorizationHandler<Handler>(this IServiceCollection services) where Handler : class, IAuthorizationHandler =>
        services.AddScoped<IAuthorizationHandler, Handler>();


    //-----------------------//


    public static IServiceCollection AddExtraPolicies(this IServiceCollection services, Action<AuthorizationOptions> configure) =>
        services.AddAuthorization(configure);


    //-----------------------//


    /// <summary>
    /// Add your own custom policies to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection
    /// </summary>
    /// <typeparam name="Handler">The type of auth handler that will deal with the added policy</typeparam>
    /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection to add services to.</param>
    /// <param name="config"> An action delegate to configure the provided Microsoft.AspNetCore.Authorization.AuthorizationOptions.</param>
    /// <returns>The Microsoft.Extensions.DependencyInjection.IServiceCollection so that additional calls can be chained.</returns>
    public static IServiceCollection AddPolicy<Handler>(this IServiceCollection services, Action<AuthorizationOptions> config) where Handler : class, IAuthorizationHandler
    {
        services.AddAuthorization(config);
        services.AddScoped<IAuthorizationHandler, Handler>();
        return services;
    }


    //-----------------------//


    /// <summary>
    /// Add your own custom policies to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection
    /// </summary>
    /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection to add services to.</param>
    /// <param name="config"> An action delegate to configure the provided Microsoft.AspNetCore.Authorization.AuthorizationOptions.</param>
    /// <returns>The Microsoft.Extensions.DependencyInjection.IServiceCollection so that additional calls can be chained.</returns>
    public static IServiceCollection AddPolicy(this IServiceCollection services, Action<AuthorizationOptions> config)
        => services.AddAuthorization(config);


    //-----------------------//


    public static IServiceCollection AddAuthHandler<Handler>(this IServiceCollection services) where Handler : class, IAuthorizationHandler
        => services.AddScoped<IAuthorizationHandler, Handler>();


}//Cls

