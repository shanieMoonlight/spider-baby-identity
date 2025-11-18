using FluentValidation;
using ID.OAuth.Amazon.HttpService;
using ID.OAuth.Amazon.Services;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ID.OAuth.Utils.Setup;

namespace ID.OAuth.Amazon.Setup;

public static class AmazonOAuthSetupExtensions
{
    public static IServiceCollection AddMyIdAmazonOAuth(this IServiceCollection services, IConfiguration configuration, string sectionName = "AmazonOAuth")
    {
        services.AddOptionsWithValidateOnStart<IdOAuthAmazonOptions, AmazonOauthSetupOptionsValidator>()
            .Bind(configuration.GetSection(sectionName));

        return services.AddAmazonOAuthDI();
    }

    public static IServiceCollection AddMyIdAmazonOAuth(this IServiceCollection services, Action<IdOAuthAmazonOptions> config)
    {
        services.Configure(config);
        services.AddOptionsWithValidateOnStart<IdOAuthAmazonOptions, AmazonOauthSetupOptionsValidator>();
        return services.AddAmazonOAuthDI();
    }

    public static IServiceCollection AddAmazonOAuthDI(this IServiceCollection services)
    {
        services.AddMyIdOAuthUtils();
        services.AddAmazonOAuthHttpClient();
        services.AddAmazonOAuthServices();

        var assembly = typeof(IdOAuthAmazonOptions).Assembly;
        services.AddMediatR(config => config.RegisterServicesFromAssembly(assembly));
        services.AddValidatorsFromAssembly(assembly);
        services.AddControllers().PartManager.ApplicationParts.Add(new AssemblyPart(typeof(IdOAuthAmazonOptions).Assembly));

        return services;
    }
}
