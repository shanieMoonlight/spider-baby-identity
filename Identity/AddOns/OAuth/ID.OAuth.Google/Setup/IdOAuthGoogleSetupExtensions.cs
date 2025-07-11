using FluentValidation;
using ID.Domain.Entities.AppUsers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;

namespace ID.OAuth.Google.Setup;

/// <summary>
/// Extension methods for setting up Google OAuth authentication in a Blazor application.
/// </summary>
public static class IdOAuthGoogleSetupExtensions
{
    /// <summary>
    /// Adds Google OAuth authentication to the Blazor application using the specified client ID, client secret, and configuration options.
    /// Includes  (Controllers, Handlers etc.)
    /// <para></para>
    /// This method requires an AuthenticationBuilder. AddMyId() returns an AuthenticationBuilder.
    /// </summary>
    /// <param name="builder">The authentication builder.</param>
    /// <param name="config">The configuration options for Google OAuth setup.</param>
    /// <returns>The updated authentication builder.</returns>
    public static AuthenticationBuilder AddMyIdOAuthGoogleBlazor(
        this AuthenticationBuilder builder, Action<IdOAuthGoogleOptions> config)
    {
        IdOAuthGoogleOptions setupOptions = new();
        config(setupOptions);

        return builder.AddMyIdOAuthGoogleBlazor(setupOptions);
    }

    //------------------------------------//

    /// <summary>
    /// Adds Google OAuth authentication to the Blazor application using the specified client ID, client secret, and setup options.
    /// Includes  (Controllers, Handlers etc.)
    /// <para></para>
    /// This method requires an AuthenticationBuilder. AddMyId() returns an AuthenticationBuilder.
    /// </summary>
    /// <param name="builder">The authentication builder.</param>
    /// <param name="options">The setup options for Google OAuth.</param>
    /// <returns>The updated authentication builder.</returns>
    public static AuthenticationBuilder AddMyIdOAuthGoogleBlazor(
        this AuthenticationBuilder builder, IdOAuthGoogleOptions options)
    {
        builder.Services.AddMyIdOAuthGoogle(options);

        builder.AddGoogle(gglOpts =>
        {
            gglOpts.ClientId = options.ClientId;
            gglOpts.ClientSecret = options.ClientId;
        });

        return builder;
    }

    //------------------------------------//

    /// <summary>
    /// Adds the necessary services (Controllers, Handlers etc.) for Google OAuth authentication to the service collection using the specified configuration options.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="config">The configuration options for Google OAuth setup.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddMyIdOAuthGoogle(this IServiceCollection services,
        Action<IdOAuthGoogleOptions>? config = null)
    {
        IdOAuthGoogleOptions setupOptions = new();
        config?.Invoke(setupOptions);

        return services.AddMyIdOAuthGoogle(setupOptions);
    }

    //------------------------------------//

    /// <summary>
    /// Adds the necessary services (Controllers, Handlers etc.) for Google OAuth authentication to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="setupOptions">The setup options for Google OAuth.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddMyIdOAuthGoogle(
        this IServiceCollection services, IdOAuthGoogleOptions? setupOptions)
    {

        var assembly = typeof(IdGoogleOAuthAssemblyReference).Assembly;
        IdOAuthGoogleOptionsSetup.ConfigureIdOAuthGoogleOptions(services, setupOptions ?? new IdOAuthGoogleOptions());

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(assembly);
        });

        // Add FluentValidation validators from the Customers assembly
        services.AddValidatorsFromAssembly(assembly);

        services.AddControllers()
            .PartManager.ApplicationParts.Add(new AssemblyPart(typeof(IdGoogleOAuthAssemblyReference).Assembly));

        services.RegisterServices<AppUser>();


        return services;
    }

    //------------------------------------//




}//Cls

