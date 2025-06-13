namespace ID.Email.Base.Setup;

public class IdEmailBaseOptions
{
    /// <summary>
    /// The URL for the company/application logo to be used in emails.
    /// </summary>
    public string? LogoUrl { get; set; }

    /// <summary>
    /// The email address that emails will be sent from. REQUIRED.
    /// </summary>
    public string FromAddress { get; set; } = string.Empty;

    /// <summary>
    /// The list of email addresses that emails will be sent to.
    /// </summary>
    public IEnumerable<string> ToAddresses { get; set; } = [];

    /// <summary>
    /// The display name for the sender. If not provided, will use application name.
    /// </summary>
    public string FromName { get; set; } = string.Empty;

    /// <summary>
    /// The list of email addresses to be CC'd on emails.
    /// </summary>
    public IEnumerable<string> CcAddresses { get; set; } = [];

    /// <summary>
    /// The list of email addresses to be BCC'd on emails.
    /// </summary>
    public IEnumerable<string> BccAddresses { get; set; } = [];

    /// <summary>
    /// The brand color in hexadecimal format for email styling. Defaults to "#0096c7".
    /// </summary>
    public string ColorHexBrand { get; set; } = MyIdEmailDefaultValues.COLOR_HEX_BRAND;
}
