using ID.Email.Base.LocalAbs;
using ID.Email.Base.Models;
using ID.Email.Base.Setup;
using ID.GlobalSettings.Setup.Options;

namespace ID.Email.Base.LocalImps.Specs.Subscriptions;

internal sealed class SubscriptionPausedSpec(string toName, string toAddress, string subPlanName, string subject = "Subscription Paused") : IEmailSpec
{
    private const string _template_path = @"Assets\html-templates\Subs\IdSubPaused.html";

    public async Task<IEmailDetails> BuildAsync(IdGlobalOptions globalOptions, ITemplateHelpers templateHelpers, IdEmailBaseOptions emailOptions)
    {
        var message = await templateHelpers.ReadAndReplaceTemplateAsync(_template_path, new Dictionary<string, string>
        {
            { EmailPlaceholders.PLACEHOLDER_USERNAME, toName },
            { EmailPlaceholders.PLACEHOLDER_SUB_PLAN_NAME, subPlanName }
        });

        return new EmailDetails(
            EmailType.HTML,
            message,
            subject,
            toAddress,
            emailOptions.BccAddresses,
            emailOptions.FromAddress,
            emailOptions.FromName
        );
    }
}
