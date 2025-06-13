using ID.Email.Base.AppAbs;
using ID.Email.Base.Models;

namespace ID.Email.Base.AppImps;

/// <summary>
/// The user will fill these out before sending email.
/// If not then the default values will be used.
/// </summary>
public class EmailDetails(
    EmailType type,
    string message,
    string subject,
    IEnumerable<string> toAddresses,
    IEnumerable<string> bccAddresses,
    string fromAddress,
    string fromName,
    IEnumerable<InlineImage>? inlineImages = null) : IEmailDetails
{

    /// <summary>
    /// Body/Content of email
    /// </summary>
    public string Message { get; set; } = message;

    /// <summary>
    /// What the email is about
    /// </summary>
    public string Subject { get; set; } = subject;

    /// <summary>
    /// The people receiving the email
    /// </summary>
    public IEnumerable<string> ToAddresses { get; set; } = toAddresses.Where(address => !string.IsNullOrWhiteSpace(address));

    /// <summary>
    /// Address of sender
    /// </summary>
    public string FromAddress { get; set; } = fromAddress;

    /// <summary>
    /// Name of sender
    /// </summary>
    public string FromName { get; set; } = fromName;

    /// <summary>
    /// Other people to include on the email 
    /// </summary>
    public IEnumerable<string> CcAddresses { get; set; } = [];

    /// <summary>
    /// Other people to include on the email (invisibly)
    /// </summary>
    public IEnumerable<string> BccAddresses { get; set; } = bccAddresses.Where(address => !string.IsNullOrWhiteSpace(address));

    /// <summary>
    /// What type of email is being sent (Text, Html, Template) - Default is TEXT
    /// </summary>
    public EmailType Type { get; set; } = type;

    /// <summary>
    /// Any inline images that will be included in in the email.
    /// </summary>
    public List<InlineImage> InlineImages { get; set; } = inlineImages?.ToList() ?? [];


    //-------------------------------------//

    public EmailDetails(
        EmailType type,
        string message,
        string subject,
        string toAddress,
        IEnumerable<string> bccAddresses,
        string fromAddress,
        string fromName,
        IEnumerable<InlineImage>? inlineImages = null)
        : this(type, message, subject, [toAddress], bccAddresses, fromAddress, fromName, inlineImages)
    { }

    //-------------------------------------//

    public IEmailDetails AddInlineImage(string filename, string contentId)
    {
        InlineImages.Add(new InlineImage(filename, contentId));
        return this;
    }

    //-------------------------------------//

}//Cls
