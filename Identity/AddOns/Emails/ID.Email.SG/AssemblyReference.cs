using System.Reflection;

namespace ID.Email.SG;

/// <summary>
/// Class for finding assembly in tests
/// </summary>
public static class IdEmailSgAssemblyReference
{
    /// <summary>
    /// Project Assemble
    /// </summary>
    public static readonly Assembly Assembly = typeof(IdEmailSgAssemblyReference).Assembly;
}
