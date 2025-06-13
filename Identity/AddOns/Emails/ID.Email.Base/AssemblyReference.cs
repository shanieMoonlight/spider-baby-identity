using System.Reflection;

namespace ID.Email.Base;

/// <summary>
/// Class for finding assembly in tests
/// </summary>
public static class IdEmailBaseAssemblyReference
{
    /// <summary>
    /// Project Assemble
    /// </summary>
    public static readonly Assembly Assembly = typeof(IdEmailBaseAssemblyReference).Assembly;
}
