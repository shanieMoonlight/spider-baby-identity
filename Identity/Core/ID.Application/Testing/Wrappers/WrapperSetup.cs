using ID.GlobalSettings.Testing.Wrappers.DirectoryClass;
using ID.GlobalSettings.Testing.Wrappers.FileClass;
using ID.GlobalSettings.Testing.Wrappers.PathClass;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ID.GlobalSettings.Testing.Wrappers;

internal static class WrapperSetup
{
    /// <summary>
    /// These are rappers around Helper classes that are used in the application.
    /// SO that they can be teste/mocked easier
    /// </summary>
    /// <param name="services"></param>
    public static IServiceCollection AddWrappers(this IServiceCollection services)
    {
        services.TryAddScoped<IPathWrapper, PathWrapper>();
        services.TryAddScoped<IDirectoryWrapper, DirectoryWrapper>();
        services.TryAddScoped<IFileWrapper, FileWrapper>();
        return services;
    }
}
