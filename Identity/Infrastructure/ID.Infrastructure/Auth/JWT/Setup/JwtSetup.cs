using ID.Application.JWT;
using ID.Domain.Claims.Utils;
using ID.GlobalSettings.Setup.Defaults;
using ID.Infrastructure.Auth.JWT.AppServiceImps;
using ID.Infrastructure.Auth.JWT.LocalServices.Abs;
using ID.Infrastructure.Auth.JWT.LocalServices.Imps;
using ID.Infrastructure.Setup;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ID.Infrastructure.Auth.JWT.Setup;
internal static class JwtSetup
{

    internal static IServiceCollection AddMyIdJwt(this IServiceCollection services, IdInfrastructureSetupOptions setupOptions)
    {

        services.ConfigureJwtOptions(setupOptions);

        services.AddJwtServices();

        return services;
    }

    //-------------------------------------//  


    /// <summary>
    /// Configures JWT options by binding from configuration.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">The configuration instance to bind from. Can be root config or a specific section.</param>
    /// <param name="sectionName">Optional section name. If null, assumes configuration is already the JWT section.</param>
    /// <returns>The service collection for method chaining.</returns>
    internal static IServiceCollection AddMyIdJwt(this IServiceCollection services, IConfiguration configuration, string? sectionName = null)
    {
        services.ConfigureJwtOptions(configuration, sectionName);

        services.AddJwtServices();

        return services;
    }


    //-------------------------------------//    

    internal static AuthenticationBuilder UseJwtAuth(this AuthenticationBuilder authBuilder)
    {
        // Register a configuration provider that will resolve services LATER
        authBuilder.Services.AddSingleton<IConfigureOptions<JwtBearerOptions>>(serviceProvider =>
        {
            return new ConfigureNamedOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                // Service resolution happens WHEN OPTIONS ARE REQUESTED, not during config
                // Will be called once then the result wil  be cached
                var jwtOptions = serviceProvider.GetRequiredService<IOptions<JwtOptions>>().Value;
                var keyHelper = serviceProvider.GetRequiredService<IKeyHelper>();

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = keyHelper.BuildValidationSigningKey(),
                    ValidIssuer = jwtOptions.TokenIssuer ?? IdGlobalDefaultValues.TOKEN_ISSUER,
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    NameClaimType = MyIdClaimTypes.NAME,
                    RoleClaimType = MyIdClaimTypes.ROLE
                };

                options.SaveToken = true;
                options.Events = MyIdJwtBearerEvents.CreateCustomEvents();
            });
        });

        authBuilder.AddJwtBearer(); // No configuration here - it uses the registered IConfigureOptions
        return authBuilder;
    }

    //-------------------------------------//   

    private static IServiceCollection AddJwtServices(this IServiceCollection services)
    {
        services.TryAddTransient<IJwtPackageProvider, JwtPackageProvider>();
        services.TryAddTransient<IJwtKeyService, JwtKeyService>();

        services.TryAddTransient<IJwtBuilder, JwtBuilder>();
        services.TryAddTransient<IJwtUtils, JwtUtils>();
        services.TryAddTransient<IJwtClaimsService, JwtClaimsService>();
        services.TryAddTransient<IKeyHelper, KeyHelper>();

        return services;
    }


    //-------------------------------------//    


}//Cls
