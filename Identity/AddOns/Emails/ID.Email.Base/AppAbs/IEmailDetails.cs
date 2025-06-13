using ID.Email.Base.Models;

namespace ID.Email.Base.AppAbs;

/// <summary>
/// Contains all the basic info that is needed to send an email
/// </summary>
public interface IEmailDetails
{
    /// <summary>
    ///Body/Content of email
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// What the email is about
    /// </summary>
    public string Subject { get; set; }

    /// <summary>
    /// The person receiving the email
    /// </summary>
    public IEnumerable<string> ToAddresses { get; set; }

    /// <summary>
    /// Address of sender
    /// </summary>
    public string FromAddress { get; set; }

    /// <summary>
    /// Name of sender
    /// </summary>
    public string FromName { get; set; }

    /// <summary>
    /// Other people to include on the email 
    /// </summary>
    public IEnumerable<string> CcAddresses { get; set; }

    /// <summary>
    /// Other people to include on the email (invisibly)
    /// </summary>
    public IEnumerable<string> BccAddresses { get; set; }



    /// <summary>
    /// What type of email is being sent (Text, Html, Template) - Default is TEXT
    /// </summary>
    public EmailType Type { get; set; }


    /// <summary>
    /// Any inline images that will be included in in the email.
    /// </summary>
    public List<InlineImage> InlineImages { get; set; }

    IEmailDetails AddInlineImage(string filename, string contentId);

}//Cls