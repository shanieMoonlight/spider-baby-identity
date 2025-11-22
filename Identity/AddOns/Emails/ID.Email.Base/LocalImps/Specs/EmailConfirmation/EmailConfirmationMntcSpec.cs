using ID.Email.Base.LocalAbs;
using ID.Email.Base.Setup;
using ID.GlobalSettings.Setup.Options;

namespace ID.Email.Base.LocalImps.Specs.EmailConfirmation;

internal sealed class EmailConfirmationMntcSpec(string toName, string toAddress, string callbackUrl) : IEmailSpec
{
    private const string _template_path = @"Assets\html-templates\EmailConfirmation\IdEmailConfirmationEmployee.html";

    public Task<IEmailDetails> BuildAsync(IdGlobalOptions globalOptions, ITemplateHelpers templateHelpers, IdEmailBaseOptions emailOptions)
    {
        return templateHelpers.GenerateTemplateWithCallback(
            toName,
            toAddress,
            callbackUrl,
            _template_path,
            $"New User - {globalOptions.ApplicationName}"
        );
    }
}
