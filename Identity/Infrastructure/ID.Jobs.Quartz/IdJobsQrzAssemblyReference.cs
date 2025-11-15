using System.Reflection;

namespace ID.Jobs.Quartz;

/// <summary>
/// Provides assembly reference for testing and reflection scenarios.
/// </summary>
public static class IdJobsQrzAssemblyReference
{
    /// <summary>
    /// Gets the ID.Application assembly reference.
    /// </summary>
    public static readonly Assembly Assembly = typeof(IdJobsQrzAssemblyReference).Assembly;
}
