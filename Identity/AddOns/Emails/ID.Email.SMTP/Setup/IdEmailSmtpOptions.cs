using ID.Email.Base.Setup;

namespace ID.Email.SMTP.Setup;

/// <summary>
/// Options for SMTP email service configuration.
/// </summary>
public class IdEmailSmtpOptions : IdEmailBaseOptions
{
    /// <summary>
    /// The SMTP server address.
    /// </summary>
    public string? SmtpServerAddress { get; set; }

    /// <summary>
    /// The SMTP port number.
    /// </summary>
    public int SmtpPortNumber { get; set; }

    /// <summary>
    /// The SMTP username or email address.
    /// </summary>
    public string? SmtpUsernameOrEmail { get; set; }

    /// <summary>
    /// The SMTP password.
    /// </summary>
    public string? SmtpPassword { get; set; }
}
