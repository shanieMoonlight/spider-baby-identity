using ID.Application.AppAbs.ExtraClaims;
using ID.Application.Models;
using ID.Application.Setup;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Setup;
using ID.GlobalSettings.Setup;
using ID.Infrastructure.Setup;
using ID.IntegrationEvents.Setup;
using ID.Persistence.Ef.Postgres;
using ID.Persistence.Ef.SQL;
using ID.Presentation.Setup;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;


namespace ID.API.Setup;

/// <summary>
/// Extension methods for setting up MyId services and middleware in an ASP.NET Core application.
/// </summary>
public static partial class IdApiSetupExtensions
{

    /// <summary>
    /// Adds MyId services to the specified IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <param name="setupOptions">The IdApiSetupOptions to configure the services.</param>
    /// <returns>The IServiceCollection with MyId services added.</returns>
    private static AuthenticationBuilder AddMyId(this IServiceCollection services, DatabaseType databaseType, IdApiSetupOptions setupOptions) =>
        services.AddMyId<DefaultExtraClaimsGenerator>(databaseType, setupOptions);

    //--------------------------// 

    /// <summary>
    /// Adds MyId services to the specified IServiceCollection with a custom extra claims generator.
    /// </summary>
    /// <typeparam name="TExtraClaimsGenerator">The type of the extra claims generator.</typeparam>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <param name="setupOptions">The IdApiSetupOptions to configure the services.</param>
    /// <returns>The IServiceCollection with MyId services added.</returns>
    private static AuthenticationBuilder AddMyId<TExtraClaimsGenerator>(this IServiceCollection services, DatabaseType databaseType, IdApiSetupOptions setupOptions)
        where TExtraClaimsGenerator : class, IExtraClaimsGenerator
        => services.Setup<TExtraClaimsGenerator, AppUser>(databaseType, setupOptions);

    //--------------------------// 

    /// <summary>
    /// Configures the specified IServiceCollection with the necessary MyId services.
    /// </summary>
    /// <param name="services">The IServiceCollection to configure.</param>
    /// <param name="setupOptions">The IdApiSetupOptions to configure the services.</param>
    /// <returns>The IServiceCollection with MyId services configured.</returns>
    private static AuthenticationBuilder Setup<TExtraClaimsGenerator, TUser>(this IServiceCollection services, DatabaseType databaseType, IdApiSetupOptions setupOptions) where TUser : AppUser
        where TExtraClaimsGenerator : class, IExtraClaimsGenerator
    {

        services.ConfigureGlobalSettings(setupOptions.GetGlobalSetupOptions());

        services.AddMyIdDomain();
        services.AddMyIdApplication<TUser>(setupOptions.GetApplicationSetupOptions());
        services.AddMyIdEvents(setupOptions.GetIntegrationEventsOptions());
        services.AddMyIdPresentation();

        //INfrastructure return a builder because that's where we hook up the actual auth stuff
        var builders = services.AddIdInfrastructure<TExtraClaimsGenerator>(databaseType, setupOptions.GetInfrastructureSetupOptions());
        Console.WriteLine($"Using {databaseType} for Identity persistence");
        switch (databaseType)
        {
            case DatabaseType.SqlServer:
                services.AddIdPersistenceEfSQL(builders.IdentityBuilder, setupOptions.ConnectionString);
                break;
            case DatabaseType.PostgreSql:
                services.AddIdPersistenceEfPostgres(builders.IdentityBuilder, setupOptions.ConnectionString);
                break;
            default:
                throw new NotSupportedException($"Database type {databaseType} is not supported.");
        }


        return builders.AuthenticationBuilder;
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




}//Cls
