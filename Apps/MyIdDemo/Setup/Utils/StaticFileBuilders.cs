using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.StaticFiles;

namespace MyIdDemo.Setup.Utils;
public static class StaticFileBuilders
{

    /// <summary>
    /// Basic set up of Static Files. Handles Dev and Prod Environments
    /// </summary>
    /// <param name="app">The Microsoft.AspNetCore.Builder.IApplicationBuilder to add the middleware to.</param>
    /// <param name="env"> Provides information about the web hosting environment an application is running in.</param>
    /// <returns> A reference to this instance after the operation has completed.</returns>
    public static IApplicationBuilder UseStaticFilesWithUnknownTypes(this IApplicationBuilder app, bool serveUnknownFileTypes = true)
    {

        var options = new StaticFileOptions()
        {
            ServeUnknownFileTypes = serveUnknownFileTypes,
            ContentTypeProvider = GetServiceWorkerManifestProvider()
        };

        //app.UseDefaultFiles(); // Enables serving default files like index.html
        app.UseStaticFiles(options);

        return app;

    }

    //--------------------------------//
    internal static IContentTypeProvider GetServiceWorkerManifestProvider()
    {
        var provider = new FileExtensionContentTypeProvider();
        provider.Mappings[".webmanifest"] = "application/manifest+json";

        return provider;

    }


}//Cls
