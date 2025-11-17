using System.Reflection;

namespace ID.OAuth.Google;

/// <summary>
/// Class for finding assembly in tests
/// </summary>
public static class IdGoogleOAuthAssemblyReference
{
    /// <summary>
    /// Project Assembly
    /// </summary>
    public static readonly Assembly Assembly = typeof(IdGoogleOAuthAssemblyReference).Assembly;
}
