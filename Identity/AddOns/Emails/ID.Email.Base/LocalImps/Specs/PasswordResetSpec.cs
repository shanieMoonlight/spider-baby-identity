using ID.Email.Base.LocalAbs;
using ID.Email.Base.Setup;
using ID.GlobalSettings.Setup.Options;

namespace ID.Email.Base.LocalImps.Specs;

internal sealed class PasswordResetSpec(string toName, string toAddress, string callbackUrl) : IEmailSpec
{
    private const string _template_path = @"Assets\html-templates\ResetPassword\IdResetPassword.html";

    public Task<IEmailDetails> BuildAsync(IdGlobalOptions globalOptions, ITemplateHelpers templateHelpers, IdEmailBaseOptions emailOptions) =>
        templateHelpers.GenerateTemplateWithCallback(
            toName,
            toAddress,
            callbackUrl,
            _template_path,
            $"Password Reset - {globalOptions.ApplicationName}"
        );
}
