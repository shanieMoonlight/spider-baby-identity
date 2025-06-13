using CollectionHelpers;
using ID.Email.Base.AppAbs;
using ID.Email.Base.Models;
using ID.Email.Base.Setup;
using ID.GlobalSettings.Setup.Options;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace ID.Email.Base.AppImps;
internal class TemplateHelpers(
    IOptions<IdGlobalOptions> _globalOptionsProvider, 
    IOptions<IdEmailBaseOptions> _emailOptionsProvider)
    : ITemplateHelpers
{
    private readonly IdGlobalOptions _globalOptions = _globalOptionsProvider.Value;
    private readonly IdEmailBaseOptions _emailOptions = _emailOptionsProvider.Value;

    //------------------------------------//   


    //<inheritdoc />
    public async Task<IEmailDetails> GenerateTemplateWithCallback(
        string toName,
        string toAddress,
        string callbackUrl,
        string templatePath,
        string subject)
    {
        var placeholders = new Dictionary<string, string>
        {
            { EmailPlaceholders.PLACEHOLDER_USERNAME, toName },
            { EmailPlaceholders.PLACEHOLDER_CALLBACK_URL, callbackUrl }
        };


        var message = await ReadAndReplaceTemplateAsync(templatePath, placeholders); return new EmailDetails(
            EmailType.HTML,
            message,
            subject,
            toAddress,
            _emailOptions.BccAddresses,
            _emailOptions.FromAddress,
            _emailOptions.FromName
        );
    }

    //------------------------------------//    
    
    //<inheritdoc />
    public async Task<string> ReadAndReplaceTemplateAsync(string templatePath, Dictionary<string, string> placeholders)
    {        //Add default placeholders
        placeholders.AddRange(new Dictionary<string, string>
        {
            { EmailPlaceholders.PLACEHOLDER_APPNAME, _globalOptions.ApplicationName },
            { EmailPlaceholders.PLACEHOLDER_COLOR_PRIMARY, _emailOptions.ColorHexBrand  ?? "#70AE6E"},
            { EmailPlaceholders.PLACEHOLDER_LOGO, _emailOptions.LogoUrl ?? string.Empty },
            { EmailPlaceholders.PLACEHOLDER_CURRENT_YEAR, DateTime.Now.Year.ToString() }
        });


        if (string.IsNullOrWhiteSpace(_emailOptions.LogoUrl))
        {
            placeholders.AddRange(new Dictionary<string, string>
            {
                { EmailPlaceholders.PLACEHOLDER_LOGO_IMAGE_DISPLAY, EmailPlaceholders.Values.LOGO_DISPLAY_NONE },
                { EmailPlaceholders.PLACEHOLDER_LOGO_TEXT_DISPLAY, EmailPlaceholders.Values.LOGO_DISPLAY_INLINE },
            });
        }
        else
        {
            placeholders.AddRange(new Dictionary<string, string>
            {
                { EmailPlaceholders.PLACEHOLDER_LOGO_IMAGE_DISPLAY, EmailPlaceholders.Values.LOGO_DISPLAY_INLINE },
                { EmailPlaceholders.PLACEHOLDER_LOGO_TEXT_DISPLAY, EmailPlaceholders.Values.LOGO_DISPLAY_NONE },
            });
        }


        var buildDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var filePath = Path.Combine(buildDir!, templatePath);  //Let the consumer handle it.  . It'll be reported

        using var sr = new StreamReader(filePath);
        var template = await sr.ReadToEndAsync();

        foreach (var placeholder in placeholders)
        {
            template = template.Replace(placeholder.Key, placeholder.Value);
        }

        return template;
    }

}//Cls
