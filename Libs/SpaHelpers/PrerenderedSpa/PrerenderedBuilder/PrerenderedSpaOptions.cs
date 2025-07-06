namespace PrerenderedSpa.PrerenderedBuilder;

public class PrerenderedSpaOptions
{
    /// <summary>
    /// "index.html"
    /// </summary>
    private const string INDEX_FILE = "index.html";

    /// <summary>
    /// Gets or sets the path, relative to the application working directory,
    /// of the directory that contains the SPA source files during
    /// development. 
    /// Will default to wwwroot if not supplied
    /// </summary>
    public string? SourcePath { get; set; }

    /// <summary>
    /// Gets or sets the default file.
    /// Default = <inheritdoc cref="INDEX_FILE"/>
    /// </summary>
    public string DefaultFileName { get; set; } = INDEX_FILE;

    /// <summary>
    /// Gets or sets the filename of the file that will be served if the default file for the request was not found.
    /// Default = <inheritdoc cref="INDEX_FILE"/>
    /// </summary>
    public string FallbackFileName { get; set; } = INDEX_FILE;

    /// <summary>
    /// Where is the fall back file located.
    /// If not set will default to the base of the  <see cref="SourcePath"/>
    /// </summary>
    public string? FallbackDirectoryRelativePath { get; set; }

    /// <summary>
    /// If the file is not a recognized content-type should it be served?
    /// Default: false.
    /// </summary>
    public bool ServeUnknownFileTypes { get; set; } = false;

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
