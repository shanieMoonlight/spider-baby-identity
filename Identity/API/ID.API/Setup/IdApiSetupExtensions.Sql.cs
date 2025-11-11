using ID.Application.AppAbs.ExtraClaims;
using ID.Application.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace ID.API.Setup;

public static partial class IdApiSetupExtensions
{

    /// <summary>
    /// Adds MyId services to the specified IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <param name="config">An action to configure the IdApiSetupOptions.</param>
    /// <returns>The IServiceCollection with MyId services added.</returns>
    public static AuthenticationBuilder AddMyId_SqlServer(this IServiceCollection services, Action<IdApiSetupOptions> config)
    {
        IdApiSetupOptions setupOptions = new();
        config(setupOptions);
        return services.AddMyId(DatabaseType.SqlServer, setupOptions);
    }

    //- - - - - - - - - - - - - // 

    /// <summary>
    /// Adds MyId services to the specified IServiceCollection with a custom extra claims generator.
    /// </summary>
    /// <typeparam name="TExtraClaimsGenerator">The type of the extra claims generator.</typeparam>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <param name="config">An action to configure the IdApiSetupOptions.</param>
    /// <returns>The IServiceCollection with MyId services added.</returns>
    public static AuthenticationBuilder AddMyId_SqlServer<TExtraClaimsGenerator>(this IServiceCollection services, Action<IdApiSetupOptions> config)
        where TExtraClaimsGenerator : class, IExtraClaimsGenerator
    {
        IdApiSetupOptions setupOptions = new();
        config(setupOptions);
        return services.AddMyId<TExtraClaimsGenerator>(DatabaseType.SqlServer, setupOptions);
    }



}//Cls
