using System.Reflection;

namespace ID.Persistence.Ef.Postgres;

/// <summary>
/// Class for finding assembly in tests
/// </summary>
public static class IdPersistenceEfPostgresAssemblyReference
{
    /// <summary>
    /// Project Assembly
    /// </summary>
    public static readonly Assembly Assembly = typeof(IdPersistenceEfPostgresAssemblyReference).Assembly;


    /// <summary>
    /// Project Assembly
    /// </summary>
    public static string AssemblyName
    {
        get
        {
            // Try simple name first
            var name = Assembly.GetName()?.Name;
            if (!string.IsNullOrEmpty(name))
                return name;

            // Fallback to full name if simple name is missing
            var full = Assembly.FullName;
            if (!string.IsNullOrEmpty(full))
                // FullName includes more info; returning it is acceptable to MigrationsAssembly
                return full;

            // Last resort: throw with useful diagnostic. WIll probably never happen.
            throw new InvalidOperationException($"Unable to determine assembly name for type {typeof(IdPersistenceEfPostgresAssemblyReference).FullName}. Assembly.IsDynamic={Assembly.IsDynamic}, Assembly.FullName={Assembly.FullName ?? "<null>"}");
        }
    }

}//Cls
