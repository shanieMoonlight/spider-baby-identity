using ID.Email.Base.LocalAbs;
using MyResults;

namespace ID.Email.Base.AppAbs;

/// <summary>
/// Service for sending emails
/// </summary>
public interface IIdEmailService
{
    /// <summary>
    /// Sends emails 
    /// </summary>
    /// <param name="eDetails"></param>
    /// <returns>Emails result</returns>
    Task<BasicResult> SendEmailAsync(IEmailDetails eDetails);


}//Int