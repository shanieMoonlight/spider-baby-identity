using MyResults;
using ID.Msg.Twilio.Setup;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using ID.Application.AppAbs.Messaging;
using Microsoft.Extensions.Options;

namespace ID.Msg.Twilio;
internal class IdTwilioWhatsAppService(IOptions<IdMsgTwilioOptions> twilioOptions) : IIdWhatsAppService
{
    private readonly IdMsgTwilioOptions _twilioOptions = twilioOptions.Value;

    public async Task<BasicResult> SendMsgAsync(string number, string message)
    {
        try
        {
            // Your Account SID from Twilio.com/console
            var accountSid = _twilioOptions.TwilioId;
            // Your Auth Token from Twilio.com/console
            var authToken = _twilioOptions.TwilioPassword;

            TwilioClient.Init(accountSid, authToken);
            number = number.Replace(" ", "");
            var msgResource = await MessageResource
               .CreateAsync(
                 to: new PhoneNumber($"{IdTwConstants.WHATS_APP_PREFIX}{number}"),
                 from: new PhoneNumber($"{IdTwConstants.WHATS_APP_PREFIX}{_twilioOptions.TwilioFromNumber}"),
                 body: message);

            return BasicResult.Success(msgResource.Sid);
        }
        catch (Exception ex)
        {
            return BasicResult.Failure(ex);
        }
    }

}//Cls
