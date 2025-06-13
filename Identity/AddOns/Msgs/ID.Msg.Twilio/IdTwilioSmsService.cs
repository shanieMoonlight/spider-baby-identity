using MyResults;
using ID.Msg.Twilio.Setup;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using ID.Application.AppAbs.Messaging;
using Microsoft.Extensions.Options;

namespace ID.Msg.Twilio;
internal class IdTwilioSmsService(IOptions<IdMsgTwilioOptions> twilioOptions) : IIdSmsService
{
    private readonly IdMsgTwilioOptions _twilioOptions = twilioOptions.Value;

    public async Task<BasicResult> SendMsgAsync(string number, string message)
    {
        try
        {
            // Plug in your SMS service here to send a text message.
            // Your Account SID from Twilio.com/console
            var accountSid = _twilioOptions.TwilioId;

            // Your Auth Token from Twilio.com/console
            var authToken = _twilioOptions.TwilioPassword;

            TwilioClient.Init(accountSid, authToken);

            var msgResource = await MessageResource.CreateAsync(
                     to: new PhoneNumber(number),
                     from: new PhoneNumber(_twilioOptions.TwilioFromNumber),
                     body: message
                 );

            return BasicResult.Success(msgResource.Sid);
        }
        catch (Exception ex)
        {
            return BasicResult.Failure(ex);
        }
    }


}//Cls
