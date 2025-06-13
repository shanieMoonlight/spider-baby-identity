namespace ID.Infrastructure.Setup.SignIn;
/// <summary>
/// Options for configuring sign in.
/// </summary>
public class IdSignInOptions
{
    /// <summary>
    /// Gets or sets a flag indicating whether a confirmed email address is required to sign in. Defaults to false.
    /// <para>
    /// Default = true
    /// </para>
    /// </summary>
    /// <value>True if a user must have a confirmed email address before they can sign in, otherwise false.</value>
    public bool RequireConfirmedEmail { get; set; } = true;

    /// <summary>
    /// Gets or sets a flag indicating whether a confirmed telephone number is required to sign in. Defaults to false.
    /// <para>
    /// Default = false
    /// </para>
    /// </summary>
    /// </summary>
    /// <value>True if a user must have a confirmed telephone number before they can sign in, otherwise false.</value>
    public bool RequireConfirmedPhoneNumber { get; set; }

}


