using System.Reflection;

namespace ID.Application;

/// <summary>
/// Provides assembly reference for testing and reflection scenarios.
/// </summary>
public static class IdApplicationAssemblyReference
{
    /// <summary>
    /// Gets the ID.Application assembly reference.
    /// </summary>
    public static readonly Assembly Assembly = typeof(IdApplicationAssemblyReference).Assembly;
}
