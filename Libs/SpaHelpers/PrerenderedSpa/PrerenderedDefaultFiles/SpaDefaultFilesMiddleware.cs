using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace PrerenderedSpa.PrerenderedDefaultFiles;
internal class SpaDefaultFilesMiddleware
{
    private const string DEFAULT_FILE = "index.html";
    private const string DEFAULT_FALLBACK_PAGE_PATH = "/";

    private readonly RequestDelegate _next;

    private readonly IFileProvider _fileProvider;
    private readonly string _fallbackDirectoryPath;
    private readonly string _fallbackFileName;
    private readonly string _defaultFileName;
    private readonly bool _fallbackToNearestParent;
    private readonly bool _redirectToCanonicalUrl;

    //------------------------------------//

    /// <summary>
    /// Creates a new instance of the DefaultFilesMiddleware.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="hostingEnv">The <see cref="IWebHostEnvironment"/> used by this middleware.</param>
    /// <param name="options">The configuration options for this middleware.</param>
    public SpaDefaultFilesMiddleware(RequestDelegate next, IWebHostEnvironment hostingEnv, IOptions<SpaDefaultFilesOptions> iOpts)
    {
        ArgumentNullException.ThrowIfNull(hostingEnv);
        ArgumentNullException.ThrowIfNull(iOpts);

        if (iOpts.Value.FileProvider == null)
            throw new ArgumentNullException(nameof(iOpts), "You must supply an IFileProvider instance");

        _next = next ?? throw new ArgumentNullException(nameof(next));

        var opts = iOpts.Value;

        _fileProvider = opts.FileProvider;
        _defaultFileName = opts.DefaultFileName ?? DEFAULT_FILE;
        _fallbackDirectoryPath = SanitizeFallbackPath(opts.FallbackDirectoryPath);
        _fallbackFileName = opts.FallbackFileName ?? DEFAULT_FILE;
        _fallbackToNearestParent = opts.FallbackToNearestParentDirectory;
        _redirectToCanonicalUrl = opts.RedirectToCanonicalUrl;

    }

    //------------------------------------//

    /// <summary>
    /// This examines the request to see if it matches a configured directory, and if there are any files with the
    /// configured default names in that directory.  If so this will append the corresponding file name to the request
    /// path for a later middleware to handle.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public Task Invoke(HttpContext context)
    {
        //No endpoint or wrong method, Just skip it
        if (context.GetEndpoint() != null || !PrerenderedHelpers.IsGetOrHeadMethod(context.Request.Method))
            return _next(context);

        var path = context.Request.Path;
          
   
        //Skip regular file requests
        if (IsFileRequest(path))
            return _next(context);

        var file = _fileProvider.GetFileInfo(PrerenderedHelpers.GetPathValueWithSlash(path.Value) + _defaultFileName);

        //Use Fallback if file not found
        if (!file.Exists)
        {
            context.Request.Path = GetNearestParentOrFallbackFile(path);
            return _next(context);
        }

        // If the path matches a directory but does not end in a slash, redirect to add the slash.
        // This prevents relative links from breaking.
        if (!PrerenderedHelpers.PathEndsInSlash(context.Request.Path) && _redirectToCanonicalUrl)
        {
            PrerenderedHelpers.RedirectToPathWithSlash(context);
            return Task.CompletedTask;
        }


        // Match found, re-write the url. A later middleware will actually serve the file.
        context.Request.Path = new PathString(PrerenderedHelpers.GetPathValueWithSlash(context.Request.Path) + _defaultFileName);


        return _next(context);

    }

    //------------------------------------//

    private PathString GetNearestParentOrFallbackFile(PathString path)
    {
        if (!_fallbackToNearestParent)
            return new PathString(PrerenderedHelpers.GetPathValueWithSlash(_fallbackDirectoryPath) + _fallbackFileName);

        if (!path.HasValue || string.IsNullOrWhiteSpace(path.Value))
            return new PathString(PrerenderedHelpers.GetPathValueWithSlash(_fallbackDirectoryPath) + _fallbackFileName);

        var segments = path.Value.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length == 0)
            return new PathString(PrerenderedHelpers.GetPathValueWithSlash(_fallbackDirectoryPath) + _fallbackFileName);

        var stack = new Stack<string>(segments);
        while (stack.Count > 1)
        {
            stack.Pop();
            var parentPath = "/" + string.Join("/", stack);
            var parentFile = PrerenderedHelpers.GetPathValueWithSlash(parentPath) + _fallbackFileName;
            var file = _fileProvider.GetFileInfo(parentFile);
            if (file.Exists)
                return new PathString(parentFile);
        }

        return new PathString(PrerenderedHelpers.GetPathValueWithSlash(_fallbackDirectoryPath) + _fallbackFileName);

    }

    //------------------------------------//

    private static bool IsFileRequest(PathString subpath)
    {
        if (!subpath.HasValue || string.IsNullOrWhiteSpace(subpath.Value))
            return false;

        var segments = subpath.Value.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length == 0)
            return false;

        return segments.Last().Contains('.');

    }

    //------------------------------------//

    private static string SanitizeFallbackPath(string? fallbackPagePath)
    {
        if (string.IsNullOrWhiteSpace(fallbackPagePath))
            return DEFAULT_FALLBACK_PAGE_PATH;

        if (!fallbackPagePath!.StartsWith(DEFAULT_FALLBACK_PAGE_PATH))
            return $"/{fallbackPagePath.Trim()}";

        return fallbackPagePath;

    }

}//Cls

