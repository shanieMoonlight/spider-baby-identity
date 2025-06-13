namespace ID.Email.Base.Models;


public enum EmailType
{
    /// <summary>
    /// Just strings
    /// </summary>
    TEXT,

    /// <summary>
    /// Html formatted
    /// </summary>
    HTML,

    /// <summary>
    /// Template - Used by the likes of SendGrid
    /// </summary>
    TEMPLATE
}//Cls