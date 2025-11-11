namespace ID.Infrastructure.Persistance.EF.Setup.Options;

/// <summary>
/// Options for configuring the general infrastructure stuff.
/// this will be used to configure the password strength validators.
/// </summary>
public class EfOptions
{
    /// <summary>
    /// How to contact the Identity Database
    /// </summary>
    internal string ConnectionString { get; set; } = string.Empty;

}//Cls

