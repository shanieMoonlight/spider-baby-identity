using System.Reflection;

namespace ID.PhoneConfirmation;

/// <summary>
/// Class for finding assembly in tests and setup
/// </summary>
public static class IdPhoneConfirmationAssemblyReference
{
    /// <summary>
    /// Project Assemble
    /// </summary>
    public static readonly Assembly Assembly = typeof(IdPhoneConfirmationAssemblyReference).Assembly;
}
