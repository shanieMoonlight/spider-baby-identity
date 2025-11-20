using System.Reflection;

namespace ID.OAuth.Amazon;

/// <summary>
/// Class for finding assembly in tests
/// </summary>
public static class IdAmazonOAuthAssemblyReference
{
    /// <summary>
    /// Project Assembly
    /// </summary>
    public static readonly Assembly Assembly = typeof(IdAmazonOAuthAssemblyReference).Assembly;
}
