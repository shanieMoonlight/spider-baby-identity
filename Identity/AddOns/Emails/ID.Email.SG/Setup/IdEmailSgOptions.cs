using ID.Email.Base.Setup;

namespace ID.Email.SG.Setup;

public class IdEmailSgOptions : IdEmailBaseOptions
{
    /// <summary>
    /// SendGrid API Connection Key. REQUIRED.
    /// </summary>
    public string? ApiKey { get; set; }
}
