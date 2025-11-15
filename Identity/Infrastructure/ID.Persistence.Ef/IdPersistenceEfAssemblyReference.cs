using System.Reflection;

namespace ID.Persistence.Ef;

/// <summary>
/// Provides assembly reference for testing and reflection scenarios.
/// </summary>
public static class IdPersistenceEfAssemblyReference
{
    /// <summary>
    /// Gets the ID.Application assembly reference.
    /// </summary>
    public static readonly Assembly Assembly = typeof(IdPersistenceEfAssemblyReference).Assembly;
}
