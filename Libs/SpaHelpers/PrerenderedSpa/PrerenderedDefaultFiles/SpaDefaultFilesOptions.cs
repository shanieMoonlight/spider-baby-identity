using Microsoft.Extensions.FileProviders;

namespace PrerenderedSpa.PrerenderedDefaultFiles;
internal class SpaDefaultFilesOptions
{
    /// <summary>
    /// "index.html"
    /// </summary>
    private const string _indexFile = "index.html";


    /// <summary>
    /// Provider for the directory the Spa Prerendered files will be in.
    /// </summary>
    public IFileProvider? FileProvider { get; set; }

    /// <summary>
    /// Gets or sets the default file.
    /// Default = <inheritdoc cref="_indexFile"/>
    /// </summary>
    public string DefaultFileName { get; set; } = _indexFile;

    /// <summary>
    /// Gets or sets the filename of the file that will be served if the default file for the request was not found.
    /// Default = <inheritdoc cref="_indexFile"/>
    /// </summary>
    public string FallbackFileName { get; set; } = _indexFile;

    /// <summary>
    /// Gets or sets the Directory of the file that will be served if the default file for the request was not found.
    /// Default = "/"
    /// </summary>
    public string? FallbackDirectoryPath { get; set; }


    /// <summary>
    /// Gets or sets th whether to fallback to the nearest parent. Look for an index file in a parent folder or go straight to the fallback index.html file
    /// Default = true
    /// </summary>
    public bool FallbackToNearestParentDirectory { get; set; } = true;

    /// <summary>
    /// Gets or sets th whether an url without a trailing slash should be redirected to the canonical url with a trailing slash.
    /// Default = true
    /// </summary>
    public bool RedirectToCanonicalUrl { get; set; } = true;


}//Cls
