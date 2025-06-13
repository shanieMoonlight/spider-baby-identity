using System.Reflection;

namespace ID.GlobalSettings;

/// <summary>
/// Class for finding assembly in tests
/// </summary>
public static class IdDomainAssemblyReference
{
    /// <summary>
    /// Project Assemble
    /// </summary>
    public static readonly Assembly Assembly = typeof(IdDomainAssemblyReference).Assembly;
}
