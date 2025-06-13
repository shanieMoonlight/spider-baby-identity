using MyResults;

namespace ID.Application.AppAbs.Messaging;

//##############################################//

/// <summary>
/// For sending Sms 
/// </summary>
public interface IIdSmsService
{
    Task<BasicResult> SendMsgAsync(string number, string message);

}//Int

//##############################################//

/// <summary>
/// For sending Whatsapp
/// </summary>
public interface IIdWhatsAppService
{
    Task<BasicResult> SendMsgAsync(string number, string message);

}//Int

//##############################################//