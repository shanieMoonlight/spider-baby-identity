using System.Reflection;

namespace ID.Application.Customers;

/// <summary>
/// Class for finding assembly in tests
/// </summary>
public static class IdApplicationCustomersAssemblyReference
{
    /// <summary>
    /// Project Assembly
    /// </summary>
    public static readonly Assembly Assembly = typeof(IdApplicationCustomersAssemblyReference).Assembly;
}
