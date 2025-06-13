using System.Reflection;

namespace ID.Presentation;

/// <summary>
/// Class for finding assembly in tests
/// </summary>
public static class IdPresentationAssemblyReference
{
    /// <summary>
    /// Project Assemble
    /// </summary>
    public static readonly Assembly Assembly = typeof(IdPresentationAssemblyReference).Assembly;
}
