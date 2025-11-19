using System.Reflection;

namespace ID.OAuth.Utils;

/// <summary>
/// Class for finding assembly in tests
/// </summary>
public static class IdOAuthUtilsAssemblyReference
{
    /// <summary>
    /// Project Assembly
    /// </summary>
    public static readonly Assembly Assembly = typeof(IdOAuthUtilsAssemblyReference).Assembly;
}
