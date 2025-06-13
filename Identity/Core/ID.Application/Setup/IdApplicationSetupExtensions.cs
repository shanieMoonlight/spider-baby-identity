using FluentValidation;
using ID.Application.AppImps;
using ID.Application.Authenticators;
using ID.Application.Mediatr;
using ID.Application.Middleware;
using ID.Application.Middleware.Exceptions;
using ID.Domain.Entities.AppUsers;
using ID.GlobalSettings.Testing.Wrappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ID.Application.Setup;

public static class IdApplicationSetupExtensions
{

    
    /// <summary>
    /// Configures ID Application services including MediatR pipeline, validation, and authentication services.
    /// Registers all application-layer dependencies required for CQRS operations and cross-cutting concerns.
    /// </summary>
    /// <typeparam name="TUser">The application user type that must inherit from AppUser</typeparam>
    /// <param name="services">The service collection to configure</param>
    /// <param name="setupOptions">Configuration options for ID Application features and middleware behavior</param>
    /// <returns>The same service collection for method chaining</returns>
    /// <exception cref="ArgumentNullException">Thrown when services or setupOptions is null</exception>
    /// <remarks>
    /// </remarks>
    public static IServiceCollection AddMyIdApplication<TUser>(this IServiceCollection services, IdApplicationOptions setupOptions) where TUser : AppUser
    {
        var assembly = typeof(IdApplicationAssemblyReference).Assembly;

        services
            .AddValidatorsFromAssembly(assembly)
            .AddApplicationImplementations<TUser>()
            .AddIdentityPolicies()            
            .AddWrappers()
            .AddHttpContextAccessor()
            .AddMyIdMediatr(assembly);


        // Configure options for dependency injection
        services.ConfigureApplicationOptions(setupOptions);


        return services;
    }

    //-----------------------------//    

    /// <summary>
    /// Configures ID Application middleware pipeline for request processing and exception handling.
    /// Sets up multi-factor authentication requirements, application header detection, and global exception handling.
    /// </summary>
    /// <param name="app">The application builder to configure</param>
    /// <returns>The same application builder for method chaining</returns>
    /// <exception cref="ArgumentNullException">Thrown when app is null</exception>
    /// <remarks>
    /// Middleware is registered in the following order:
    /// 1. Multi-factor authentication middleware - Enforces MFA requirements
    /// 2. From-app middleware - Detects requests from mobile applications (conditional)
    /// 3. Custom exception handler - Converts exceptions to appropriate HTTP responses
    /// </remarks>
    public static IApplicationBuilder UseMyIdApplication(this IApplicationBuilder app)    {
        
        app.UseMultiFactorRequiredMiddleware();

        // Only register FromApp middleware if header value is configured
        var options = app.ApplicationServices.GetRequiredService<IOptions<IdApplicationOptions>>();
        if (!string.IsNullOrWhiteSpace(options.Value.FromAppHeaderValue))
            app.UseFromAppMiddleware();

        app.UseCustomExceptionHandler(new MyIdExceptionConverter());

        return app;
    }


}//Cls

