namespace ID.Email.Base.Models;

/// <summary>
/// Class for encapsulating an inline image of an email.
/// </summary>
public class InlineImage
{
    /// <summary>
    /// Where the image can be found
    /// </summary>
    public string ImagePath { get; set; } = string.Empty;

    /// <summary>
    /// Id of the image in the email
    /// </summary>
    public string ContentId { get; private set; } = string.Empty;

    /// <summary>
    /// Current format (Used in for Sendgrid, NOT used in SMTP) 
    /// </summary>
    public string Content { get; private set; } = string.Empty;

    /// <summary>
    /// Original format (png, jpg, etc.)
    /// </summary>
    public string Type { get; set; } = "image/png";

    /// <summary>
    /// 
    /// </summary>
    public string Filename { get => ImagePath; private set { ImagePath = value; } }

    /// <summary>
    /// Always return "inline"
    /// </summary>
    public static string Disposition { get => "inline"; }

    //------------------------------------//

    /// <summary>
    ///  Constructor used when adding images in Base64 Format
    /// </summary>
    /// <param name="filename">Full path to file that will be attached.</param>
    public InlineImage(string filename)
    {
        Filename = filename;
        if (File.Exists(Filename))
        {
            var bytes = File.ReadAllBytes(Filename);
            Content = Convert.ToBase64String(bytes);
        }
    }

    //------------------------------------//

    /// <summary>
    ///  Constructor used when adding images in Base64 Format
    /// </summary>
    /// <param name="filename">Full path to file that will be attached.</param>
    /// <param name="contentId">The link from html to the img File</param>
    public InlineImage(string filename, string contentId) : this(filename) =>
        ContentId = contentId;

    //------------------------------------//

}//Cls