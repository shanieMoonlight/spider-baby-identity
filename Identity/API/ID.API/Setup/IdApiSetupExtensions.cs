using ID.Application.AppAbs.ExtraClaims;
using ID.Application.AppAbs.RequestInfo;
using ID.Application.Jobs;
using ID.Application.Setup;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Domain.Setup;
using ID.GlobalSettings.Setup;
using ID.Infrastructure.Setup;
using ID.IntegrationEvents.Setup;
using ID.Presentation.Setup;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics;


namespace ID.API.Setup;

/// <summary>
/// Extension methods for setting up MyId services and middleware in an ASP.NET Core application.
/// </summary>
public static class IdApiSetupExtensions
{

    /// <summary>
    /// Adds MyId services to the specified IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <param name="config">An action to configure the IdApiSetupOptions.</param>
    /// <returns>The IServiceCollection with MyId services added.</returns>
    public static AuthenticationBuilder AddMyId(this IServiceCollection services, Action<IdApiSetupOptions> config)
    {
        IdApiSetupOptions setupOptions = new();
        config(setupOptions);
        return services.AddMyId(setupOptions);
    }

    //------------------------------------//

    /// <summary>
    /// Adds MyId services to the specified IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <param name="setupOptions">The IdApiSetupOptions to configure the services.</param>
    /// <returns>The IServiceCollection with MyId services added.</returns>
    public static AuthenticationBuilder AddMyId(this IServiceCollection services, IdApiSetupOptions setupOptions) =>
        services.AddMyId<DefaultExtraClaimsGenerator>(setupOptions);

    //------------------------------------//

    /// <summary>
    /// Adds MyId services to the specified IServiceCollection with a custom extra claims generator.
    /// </summary>
    /// <typeparam name="TExtraClaimsGenerator">The type of the extra claims generator.</typeparam>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <param name="config">An action to configure the IdApiSetupOptions.</param>
    /// <returns>The IServiceCollection with MyId services added.</returns>
    public static AuthenticationBuilder AddMyId<TExtraClaimsGenerator>(this IServiceCollection services, Action<IdApiSetupOptions> config)
        where TExtraClaimsGenerator : class, IExtraClaimsGenerator
    {
        IdApiSetupOptions setupOptions = new();
        config(setupOptions);
        return services.AddMyId<TExtraClaimsGenerator>(setupOptions);
    }

    //------------------------------------//

    /// <summary>
    /// Adds MyId services to the specified IServiceCollection with a custom extra claims generator.
    /// </summary>
    /// <typeparam name="TExtraClaimsGenerator">The type of the extra claims generator.</typeparam>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <param name="setupOptions">The IdApiSetupOptions to configure the services.</param>
    /// <returns>The IServiceCollection with MyId services added.</returns>
    public static AuthenticationBuilder AddMyId<TExtraClaimsGenerator>(this IServiceCollection services, IdApiSetupOptions setupOptions)
        where TExtraClaimsGenerator : class, IExtraClaimsGenerator
        => services.Setup<TExtraClaimsGenerator, AppUser>(setupOptions);

    //------------------------------------//

    /// <summary>
    /// Configures the specified IServiceCollection with the necessary MyId services.
    /// </summary>
    /// <param name="services">The IServiceCollection to configure.</param>
    /// <param name="setupOptions">The IdApiSetupOptions to configure the services.</param>
    /// <returns>The IServiceCollection with MyId services configured.</returns>
    private static AuthenticationBuilder Setup<TExtraClaimsGenerator, TUser>(this IServiceCollection services, IdApiSetupOptions setupOptions) where TUser : AppUser
        where TExtraClaimsGenerator : class, IExtraClaimsGenerator
    {
        
        services.ConfigureGlobalSettings(setupOptions.GetGlobalSetupOptions());

        services.AddMyIdDomain();
        services.AddMyIdApplication<TUser>(setupOptions.GetApplicationSetupOptions());
        services.AddMyIdEvents(setupOptions.GetIntegrationEventsOptions());
        services.AddMyIdPresentation();

        //INfrastructure return a builder because that's where we hook up the actual auth stuff
        var builder = services.AddIdInfrastructure<TExtraClaimsGenerator>(setupOptions.GetInfrastructureSetupOptions());
        return builder;
    }

    //------------------------------------//

    /// <summary>
    /// Adds an authorization handler to the specified IServiceCollection.
    /// </summary>
    /// <typeparam name="Handler">The type of the authorization handler.</typeparam>
    /// <param name="services">The IServiceCollection to add the handler to.</param>
    /// <returns>The IServiceCollection with the authorization handler added.</returns>
    public static IServiceCollection AddAuthHandler<Handler>(this IServiceCollection services) where Handler : class, IAuthorizationHandler
        => services.AddScoped<IAuthorizationHandler, Handler>();

    //------------------------------------//

    /// <summary>
    /// Configures the specified IApplicationBuilder to use MyId authentication and authorization.
    /// Maps controllers if not getting mapped in main app.
    /// </summary>
    /// <param name="app">The IApplicationBuilder to configure.</param>
    /// <param name="minTypeDashboardAccess">The Job dashboard is protected. This is the minimum level required to access it.</param>
    /// <returns>The IApplicationBuilder with MyId authentication and authorization configured.</returns>
    public static IApplicationBuilder UseMyIdAndMapControllers<TWebApp>(this TWebApp app, TeamType minTypeDashboardAccess = TeamType.Maintenance)
        where TWebApp : IApplicationBuilder, IEndpointRouteBuilder
    {
        app.MapMyIdControllers();
        app.UseMyId(minTypeDashboardAccess);

        return app;
    }

    //------------------------------------//

    /// <summary>
    /// Configures the specified IApplicationBuilder to use MyId authentication and authorization.
    /// </summary>
    /// <param name="app">The IApplicationBuilder to configure.</param>
    /// <param name="minTypeDashboardAccess">The Job dashboard is protected. This is the minimum level required to access it. No auth required in Dev Mode</param>
    /// <returns>The IApplicationBuilder with MyId authentication and authorization configured.</returns>
    public static IApplicationBuilder UseMyId(this IApplicationBuilder app, TeamType minTypeDashboardAccess = TeamType.Super)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        // This goes AFTER regular auth because we need the user to be authenticated or not before this point
        try
        {
            app.UseMyIdApplication();
            app.UseMyIdInfrastructure(minTypeDashboardAccess);
            var cancellationTokenSource = new CancellationTokenSource();
            app.ApplicationServices.StartRecurringMyIdJobs(cancellationTokenSource.Token);
        }
        catch (Exception e)
        {
            // This can be caused if the DB has not been initialized yet
            Debug.WriteLine(e);
            Console.WriteLine(e);
        }

        return app;
    }


}//Cls
