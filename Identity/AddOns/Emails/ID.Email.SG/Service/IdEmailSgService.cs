using CollectionHelpers;
using ID.Email.Base.Abs;
using ID.Email.Base.AppAbs;
using ID.Email.Base.Models;
using ID.Email.Base.Utility;
using ID.Email.SG.Setup;
using Microsoft.Extensions.Options;
using MyResults;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;

namespace ID.Email.SG.Service;
public class IdEmailSgService(IOptions<IdEmailSgOptions> emailOptions) : IIdEmailService
{
    private readonly IdEmailSgOptions _emailOptions = emailOptions.Value;

    //---------------------------------------//

    public async Task<BasicResult> SendEmailAsync(IEmailDetails eDetails) =>
        eDetails.Type switch
        {
            EmailType.TEXT => await SendEmailBasicAsync(_emailOptions.ApiKey!, eDetails),
            EmailType.HTML => await SendEmailBasicAsync(_emailOptions.ApiKey!, eDetails),
            _ => throw new IdEmailException(IdEmailingMessages.Error.WRONG_TYPE),
        };

    //------------------------------------//

    private static async Task<BasicResult> SendEmailBasicAsync(string apiKey, IEmailDetails eDetails)
    {
        var client = new SendGridClient(apiKey);
        var from = new EmailAddress(eDetails.FromAddress, eDetails.FromName);
        var subject = eDetails.Subject;
        var toAddresses = new List<EmailAddress>();
        var ccAddresses = new List<EmailAddress>();
        var bccAddresses = new List<EmailAddress>();


        toAddresses.AddRange(
            eDetails.ToAddresses.Select(address => new EmailAddress(address))
        );


        var content = eDetails.Message;
        SendGridMessage msg;
        if (eDetails.Type == EmailType.HTML)
            msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, toAddresses, subject, null, content);
        else
            msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, toAddresses, subject, content, null);


        AddInlineImages(msg, eDetails.InlineImages);


        var safeCcs = SanitizeCcAddresses(eDetails.CcAddresses, eDetails.ToAddresses);
        if (safeCcs.AnyValues())
        {
            ccAddresses.AddRange(
               safeCcs.Select(address => new EmailAddress(address))
            );
            msg.AddCcs(ccAddresses);

        }//if


        var safeBccs = SanitizeBccAddresses(eDetails.BccAddresses, eDetails.ToAddresses, eDetails.CcAddresses);
        if (safeBccs.AnyValues())
        {
            bccAddresses.AddRange(
                safeBccs.Select(address => new EmailAddress(address))
            );
            msg.AddBccs(bccAddresses);

        }


        return await TrySendEmailAsync(client, msg);

    }

    //------------------------------------//

    private static async Task<BasicResult> TrySendEmailAsync(SendGridClient client, SendGridMessage msg)
    {

        Response response;
        try
        {
            response = await client.SendEmailAsync(msg);

            if (response.StatusCode != HttpStatusCode.Accepted && response.StatusCode != HttpStatusCode.OK)
            {
                var bodyString = await response.Body.ReadAsStringAsync();
                return BasicResult.Failure(bodyString);
            }

            return BasicResult.Success(IdEmailingMessages.Info.SENT_SUCCESSFULLY);

        }
        catch (Exception ex)
        {
            return BasicResult.Failure(ex, $"{ex?.Message} {Environment.NewLine} {ex?.InnerException?.Message}");
        }
    }

    //------------------------------------//

    private static void AddInlineImages(SendGridMessage msg, IEnumerable<InlineImage> inlineImages)
    {
        if (inlineImages == null)
            return;

        var attachments = new List<Attachment>();
        foreach (var inlineImg in inlineImages)
        {
            attachments.Add(new Attachment()
            {
                Content = inlineImg.Content,
                Type = inlineImg.Type,
                Filename = Path.GetFileName(inlineImg.Filename),
                Disposition = InlineImage.Disposition,
                ContentId = inlineImg.ContentId
            });
        }

        if (attachments.Count > 0)
            msg.AddAttachments(attachments);

    }

    //------------------------------------//

    private static IEnumerable<string> SanitizeCcAddresses(IEnumerable<string> ccs, IEnumerable<string> tos)
    {
        if (!ccs.AnyValues() || !tos.AnyValues())
            return ccs;

        var toList = tos.ToLowerList();
        var ccList = ccs.ToLowerList();

        ccList.RemoveAll(address => toList.Contains(address));

        return ccList;

    }

    //------------------------------------//

    private static IEnumerable<string> SanitizeBccAddresses(IEnumerable<string> bccs, IEnumerable<string> tos, IEnumerable<string> ccs)
    {
        if (!bccs.AnyValues())
            return bccs;

        if (!tos.AnyValues() && !ccs.AnyValues())
            return bccs;

        var toList = tos.ToLowerList();
        var ccList = ccs.ToLowerList();
        var bccList = bccs.ToLowerList();

        bccList.RemoveAll(address => toList.Contains(address));
        bccList.RemoveAll(address => ccList.Contains(address));

        return bccList;

    }

    //------------------------------------//

}//Cls
