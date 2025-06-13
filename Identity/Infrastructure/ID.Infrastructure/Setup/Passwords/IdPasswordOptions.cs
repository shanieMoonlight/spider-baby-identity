namespace ID.Infrastructure.Setup.Passwords;

/// <summary>
/// Options for configuring the password policy.
/// this will be used to configure the password strength validators.
/// </summary>
public class IdPasswordOptions
{
    /// <summary>
    /// Gets or sets the minimum length a password must be. Defaults to 6.
    /// </summary>
    public int RequiredLength { get; set; } = IdPasswordDefaultValues.RequiredLength;

    /// <summary>
    /// Gets or sets the minimum number of unique characters which a password must contain. Defaults to 1.
    /// </summary>
    public int RequiredUniqueChars { get; set; } = IdPasswordDefaultValues.RequiredUniqueChars;

    /// <summary>
    /// Gets or sets a flag indicating if passwords must contain a non-alphanumeric character. Defaults to true.
    /// </summary>
    /// <value>True if passwords must contain a non-alphanumeric character, otherwise false.</value>
    public bool RequireNonAlphanumeric { get; set; } = IdPasswordDefaultValues.RequireNonAlphanumeric;

    /// <summary>
    /// Gets or sets a flag indicating if passwords must contain a lower case ASCII character. Defaults to true.
    /// </summary>
    /// <value>True if passwords must contain a lower case ASCII character.</value>
    public bool RequireLowercase { get; set; } = IdPasswordDefaultValues.RequireLowercase;

    /// <summary>
    /// Gets or sets a flag indicating if passwords must contain a upper case ASCII character. Defaults to true.
    /// </summary>
    /// <value>True if passwords must contain a upper case ASCII character.</value>
    public bool RequireUppercase { get; set; } = IdPasswordDefaultValues.RequireUppercase;

    /// <summary>
    /// Gets or sets a flag indicating if passwords must contain a digit. Defaults to true.
    /// </summary>
    /// <value>True if passwords must contain a digit.</value>
    public bool RequireDigit { get; set; } = IdPasswordDefaultValues.RequireDigit;
}

