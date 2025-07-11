using ID.GlobalSettings.Setup.Options;

namespace ID.Application.Features.System.Qry.Settings;

public record SettingsDto(
    IdGlobalOptions Settings, 
    IdGlobalSetupOptions_CUSTOMER CustomerSettings);