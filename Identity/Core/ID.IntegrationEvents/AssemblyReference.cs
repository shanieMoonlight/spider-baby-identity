using System.Reflection;

namespace ID.IntegrationEvents;

/// <summary>
/// Class for finding assembly in tests
/// </summary>
public static class IdIntegrationEventsAssemblyReference
{
    /// <summary>
    /// Project Assembly
    /// </summary>
    public static readonly Assembly Assembly = typeof(IdIntegrationEventsAssemblyReference).Assembly;
}
