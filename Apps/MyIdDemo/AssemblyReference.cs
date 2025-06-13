using System.Reflection;

namespace MyIdDemo;

/// <summary>
/// Class for finding assembly in tests
/// </summary>
public static class MyIdDemoAssemblyReference
{
    /// <summary>
    /// Project Assemble
    /// </summary>
    public static readonly Assembly Assembly = typeof(MyIdDemoAssemblyReference).Assembly;
}
