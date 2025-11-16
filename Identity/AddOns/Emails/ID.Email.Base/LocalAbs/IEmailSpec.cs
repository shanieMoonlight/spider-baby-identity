using ID.Email.Base.Setup;
using ID.GlobalSettings.Setup.Options;

namespace ID.Email.Base.LocalAbs;

/// <summary>
/// Lightweight POCO specification for building an email.
/// Implementations hold only runtime data (toName, toAddress, tokens, etc.)
/// and rely on the generator to supply shared services/options.
/// </summary>
public interface IEmailSpec
{
    Task<IEmailDetails> BuildAsync(IdGlobalOptions globalOptions, ITemplateHelpers templateHelpers, IdEmailBaseOptions emailOptions);
}
