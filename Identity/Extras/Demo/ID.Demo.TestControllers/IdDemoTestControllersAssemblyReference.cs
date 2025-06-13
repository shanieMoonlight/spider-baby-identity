using System.Reflection;

namespace ID.Demo.TestControllers;

/// <summary>
/// Class for finding assembly in tests
/// </summary>
public static class IdDemoTestControllersAssemblyReference
{
    /// <summary>
    /// Project Assemble
    /// </summary>
    public static readonly Assembly Assembly = typeof(IdDemoTestControllersAssemblyReference).Assembly;
}
