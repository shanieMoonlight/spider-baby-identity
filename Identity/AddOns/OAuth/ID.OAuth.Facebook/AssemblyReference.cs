using System.Reflection;

namespace ID.OAuth.Facebook;

/// <summary>
/// Class for finding assembly in tests
/// </summary>
public static class IdFacebookOAuthAssemblyReference
{
    /// <summary>
    /// Project Assembly
    /// </summary>
    public static readonly Assembly Assembly = typeof(IdFacebookOAuthAssemblyReference).Assembly;
}
