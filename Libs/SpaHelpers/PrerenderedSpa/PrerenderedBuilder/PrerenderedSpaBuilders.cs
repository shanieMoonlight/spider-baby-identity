using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using PrerenderedSpa.PrerenderedDefaultFiles;

namespace PrerenderedSpa.PrerenderedBuilder;
public static class PrerenderedSpaBuilders
{

    /// <summary>
    /// Setup the serving of SPA prerendered files
    /// </summary>
    /// <param name="app">The Microsoft.AspNetCore.Builder.IApplicationBuilder to add the middleware to.</param>
    /// <param name="spaRootFolderName"> Where are the SPA files located.</param>
    /// <returns> A reference to this instance after the operation has completed.</returns>
    public static IApplicationBuilder UsePrerenderedSpa(this IApplicationBuilder app, PrerenderedSpaOptions? options = null)
    {

        options ??= new PrerenderedSpaOptions();
        var env = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();
        IFileProvider fileProvider = env.WebRootFileProvider;
        if (!string.IsNullOrWhiteSpace(options.SourcePath))
        {
            string fullSpaRootPath = Path.Combine(env.ContentRootPath, options.SourcePath!);
            fileProvider = new PhysicalFileProvider(fullSpaRootPath);
        }


        app.UsePrerenderedSpaDefaultFiles(new SpaDefaultFilesOptions()
        {
            FileProvider = fileProvider,
            FallbackFileName = options.FallbackFileName,
            FallbackDirectoryPath = options.FallbackDirectoryRelativePath,
            DefaultFileName = options.DefaultFileName,
            FallbackToNearestParentDirectory = options.FallbackToNearestParentDirectory,
            RedirectToCanonicalUrl = options.RedirectToCanonicalUrl,
        });


        app.UseStaticFiles(new StaticFileOptions()
        {
            ServeUnknownFileTypes = options.ServeUnknownFileTypes,
            ContentTypeProvider = ServiceWorkerManifestProvider,
            FileProvider = fileProvider,
        });

        return app;

    }


    //- - - - - - - - - - - - - - - - //
    

    /// <summary>
    /// Setup the serving of SPA prerendered files
    /// </summary>
    /// <param name="app">The Microsoft.AspNetCore.Builder.IApplicationBuilder to add the middleware to.</param>
    /// <param name="sourcePath">The path, relative to the application working directory,
    /// of the directory that contains the SPA source files during</param>
    /// <returns> A reference to this instance after the operation has completed.</returns>
    public static IApplicationBuilder UsePrerenderedSpa(this WebApplication app, string sourcePath) =>
        app.UsePrerenderedSpa(new PrerenderedSpaOptions()
        {
            SourcePath = sourcePath
        });


    //--------------------------------//  


    private static IContentTypeProvider ServiceWorkerManifestProvider
    {
        get
        {
            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".webmanifest"] = "application/manifest+json";

            return provider;

        }
    }

    


}//Cls
