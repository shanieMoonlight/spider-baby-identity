using System.Reflection;

namespace ID.Email.SMTP;

/// <summary>
/// Class for finding assembly in tests
/// </summary>
public static class IdEmailSmtpAssemblyReference
{
    /// <summary>
    /// Project Assemble
    /// </summary>
    public static readonly Assembly Assembly = typeof(IdEmailSmtpAssemblyReference).Assembly;
}
