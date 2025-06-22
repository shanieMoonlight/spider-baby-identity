using System.Reflection;

namespace ID.Infrastructure;

/// <summary>
/// Class for finding assembly in tests
/// </summary>
public static class IdInfrastructureAssemblyReference
{
    /// <summary>
    /// Project Assemble
    /// </summary>
    public static readonly Assembly Assembly = typeof(IdInfrastructureAssemblyReference).Assembly;
}
