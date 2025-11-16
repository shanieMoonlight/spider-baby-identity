using ID.Email.Base.LocalAbs;
using ID.Email.Base.Setup;
using ID.GlobalSettings.Setup.Options;
using Microsoft.Extensions.Options;

namespace ID.Email.Base.LocalImps;

internal class EmailDetailsTemplateGenerator(
    IOptions<IdGlobalOptions> _globalOptionsProvider,
    ITemplateHelpers _templateHelpers,
    IOptions<IdEmailBaseOptions> _emailOptionsProvider)
    : IEmailDetailsTemplateGenerator
{

    public Task<IEmailDetails> GenerateFromSpecAsync(IEmailSpec spec) =>
        spec is null
            ? throw new ArgumentNullException(nameof(spec))
            : spec.BuildAsync(
                globalOptions: _globalOptionsProvider.Value, 
                templateHelpers: _templateHelpers, 
                emailOptions: _emailOptionsProvider.Value);

}

