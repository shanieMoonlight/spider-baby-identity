using ID.Email.Base.Abs;
using ID.Email.Base.AppAbs;
using ID.Email.Base.Models;
using ID.Email.Base.Utility;
using ID.Email.SMTP.Setup;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MyResults;
using System.Diagnostics;

namespace ID.Email.SMTP.Service;
internal class IdEmailSmtpService(IOptions<IdEmailSmtpOptions> options) : IIdEmailService
{
    private readonly IdEmailSmtpOptions _options = options.Value;
    /// <summary>
    /// Sends emails.
    /// </summary>
    /// <param name="eDetails"></param>
    /// <returns>Task of EmailResult with details of how it went</returns>
    public async Task<BasicResult> SendEmailAsync(IEmailDetails eDetails) =>
        eDetails.Type switch
        {
            EmailType.TEXT => await SendTextEmailAsync(eDetails),
            EmailType.HTML => await SendHtmlEmailAsync(eDetails),
            _ => BasicResult.Failure(IdEmailingMessages.Error.WRONG_TYPE),
        };

    //-------------------------------------//

    /// <summary>
    /// Sends Text only emails.
    /// </summary>
    public async Task<BasicResult> SendTextEmailAsync(IEmailDetails eDetails)
    {
        var body = new TextPart("plain")
        {
            Text = eDetails.Message
        };

        return await SendEmailAsync(eDetails, body);
    }

    //-------------------------------------//    /// <summary>
    /// Sends emails.
    /// </summary>
    /// <param name="eDetails"></param>
    /// <returns></returns>
    public async Task<BasicResult> SendHtmlEmailAsync(IEmailDetails eDetails)
    {

        var builder = new BodyBuilder();

        if (eDetails.InlineImages != null)
        {
            var inlineImages = eDetails.InlineImages.ToList();
            for (int i = 0; i < inlineImages.Count; i++)
            {
                if (!File.Exists(inlineImages[i].ImagePath))
                    continue;

                var attachedImage = builder.LinkedResources.Add(inlineImages[i].ImagePath);
                attachedImage.ContentId = inlineImages[i].ContentId;
            }
        }


        builder.HtmlBody = eDetails.Message;
        var body = builder.ToMessageBody();

        return await SendEmailAsync(eDetails, body);
    }

    //-------------------------------------//

    private async Task<BasicResult> SendEmailAsync(IEmailDetails eDetails, MimeEntity body)
    {
        var toAddresses = ConvertToMailBoxAddress(eDetails.ToAddresses);
        var ccAddresses = ConvertToMailBoxAddress(eDetails.CcAddresses);
        var bccAddresses = ConvertToMailBoxAddress(eDetails.BccAddresses);
        //
        var mimeMessage = new MimeMessage();
        mimeMessage.From.Add(new MailboxAddress(eDetails.FromName, _options.SmtpUsernameOrEmail));
        mimeMessage.To.AddRange(toAddresses);
        mimeMessage.Cc.AddRange(ccAddresses);
        mimeMessage.Bcc.AddRange(bccAddresses);
        mimeMessage.Subject = eDetails.Subject;

        mimeMessage.Body = body;

        try
        {
            using var client = new SmtpClient
            {
                ServerCertificateValidationCallback = (s, c, h, e) => true
            };

            client.Connect(_options.SmtpServerAddress, _options.SmtpPortNumber, SecureSocketOptions.Auto);
            client.AuthenticationMechanisms.Remove("XOAUTH2");
            client.Authenticate(_options.SmtpUsernameOrEmail, _options.SmtpPassword);


            var serverResponse = await client.SendAsync(mimeMessage);
            client.Disconnect(true);

            Debug.WriteLine($"Message: {eDetails.Message} - {serverResponse}");

            return BasicResult.Success(IdEmailingMessages.Info.SENT_SUCCESSFULLY);

        }
        catch (Exception ex)
        {
            return BasicResult.Failure(ex, IdEmailingMessages.Error.SEND_FAILURE_GENERAL);
        }
    }

    //-------------------------------------//

    private static List<MailboxAddress> ConvertToMailBoxAddress(IEnumerable<string> addresses)
    {
        if (addresses == null)
            return [];

        var toAddresses = new List<MailboxAddress>();
        foreach (var address in addresses)
            toAddresses.Add(MailboxAddress.Parse(address));

        return toAddresses;
    }


}//Cls
