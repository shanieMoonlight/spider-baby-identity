using Microsoft.AspNetCore.StaticFiles;

namespace MyIdDemo.Setup.Utils;
public static class SpaStaticFileBuilders
{

    /// <summary>
    /// Basic set up of Static Files. Handles Dev and Prod Environments
    /// </summary>
    /// <param name="app">The Microsoft.AspNetCore.Builder.IApplicationBuilder to add the middleware to.</param>
    /// <param name="serveUnknownFileTypes">Indicates whether to serve files with unknown MIME types.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IApplicationBuilder UseSpaStaticFilesWithUnknownTypes(this IApplicationBuilder app, bool serveUnknownFileTypes = true)
    {

        var options = new StaticFileOptions()
        {
            ServeUnknownFileTypes = serveUnknownFileTypes,
            ContentTypeProvider = GetServiceWorkerManifestProvider()
        };


        app.UseSpaStaticFiles(options);


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
