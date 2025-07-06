using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

namespace PrerenderedSpa.PrerenderedDefaultFiles;
public static class SpaDefaultFilesExtensions
{

    /// <summary>
    /// Enables default file mapping with the given options
    /// </summary>
    /// <param name="app"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    internal static IApplicationBuilder UsePrerenderedSpaDefaultFiles(this IApplicationBuilder app, SpaDefaultFilesOptions options)
    {
        ArgumentNullException.ThrowIfNull(app);
        ArgumentNullException.ThrowIfNull(options);

        return app.UseMiddleware<SpaDefaultFilesMiddleware>(Options.Create(options));

    }


}//Cls
