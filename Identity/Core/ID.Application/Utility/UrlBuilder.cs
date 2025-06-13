using System.Text;

namespace ID.GlobalSettings.Utility;

/// <summary>
/// Utility for safely combining URL segments with proper formatting.
/// </summary>
public class UrlBuilder
{
    /// <summary>
    /// Combines multiple URL segments into a single, properly formatted URL path.
    /// </summary>
    /// <param name="segments">URL segments to combine</param>
    /// <returns>Combined URL path with forward slashes, trimmed of leading/trailing slashes</returns>
    public static string Combine(params string[] segments)
    {

        var urlSb = new StringBuilder();
        var safeSegments = (segments ?? [])
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Where(s => s != "/")
            .Where(s => s != "\\")
            .Select(s => s.ToLower());

        return string
            .Join("/", safeSegments)
            .Trim('/');
    }
}
