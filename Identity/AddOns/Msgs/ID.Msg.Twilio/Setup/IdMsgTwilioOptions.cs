using Microsoft.Extensions.Configuration;

namespace ID.Msg.Twilio.Setup;

public class IdMsgTwilioOptions
{


    public string? TwilioId { get; set; }
    public string? TwilioPassword { get; set; }
    public string? TwilioFromNumber { get; set; }


    /// <summary>
    /// Configuration section containing api keys, ids, etc.
    /// </summary>
    public IConfigurationSection? MsgConfig { get; set; }
    public string? MsgConfigSectionName{ get; set; }


}//Cls
