using ID.PhoneConfirmation.Setup;

namespace ID.PhoneConfirmation.Events.Integration;

internal class PhoneConfirmationMsgTemplateGenerator
{  public  static string Generate(string username, string link, string applicationName) => $@"
Hi {username},

    You're receiving this text message because you're registered as a user of {applicationName}.
    Click the link below to confirm your phone number.

    {link}

Thanks
The Team at {applicationName}
";

}