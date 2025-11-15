using ID.Email.Base.AppAbs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ID.Email.Base.Cache;
internal static class EmailBaseCachingSetup
{
    public static IServiceCollection AddEmailTemplateCaching(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddOptions<TemplateCacheOptions>()
            .Configure(c => { });

        services.TryAddSingleton<TemplateCacheInvalidator>();


        // Decorate the TemplateLoader with in-memory caching
        services.Decorate<ITemplateLoader, TemplateLoaderCache_InMemory>();

        return services;
    }
}
