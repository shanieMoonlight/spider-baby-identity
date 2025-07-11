using ID.Application.Mediatr.CqrsAbs;
using ID.GlobalSettings.Setup.Options;
using Microsoft.Extensions.Options;
using MyResults;

namespace ID.Application.Features.System.Qry.Settings;
public class GetSettingsCmdHandler(
    IOptions<IdGlobalOptions> _globalOptionsProvider,
    IOptions<IdGlobalSetupOptions_CUSTOMER> _globalCustomerOptionsProvider)
    : IIdCommandHandler<GetSettingsCmd, SettingsDto>
{

    public Task<GenResult<SettingsDto>> Handle(GetSettingsCmd request, CancellationToken cancellationToken)
    {

        var dto = new SettingsDto(
            _globalOptionsProvider.Value,
            _globalCustomerOptionsProvider.Value);

        return Task.FromResult(GenResult<SettingsDto>.Success(dto));
    }



}//Cls

