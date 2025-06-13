using System.ComponentModel;

namespace ID.Domain.Models;

public enum TwoFactorProvider
{
    [Description("Email")]
    Email = 1,
    [Description("Sms")]
    Sms = 2,
    [Description("AuthenticatorApp")]
    AuthenticatorApp = 3,
    //[Description("Authy")]
    //Authy = 3,
    //[Description("WhatsApp")]
    //WhatsApp = 3,
    //[Description("MsAuthenticator")]
    //MsAuthenticator = 5,

}//Enm