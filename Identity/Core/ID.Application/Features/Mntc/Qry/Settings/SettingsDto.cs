using ID.GlobalSettings.Setup.Options;

namespace ID.Application.Features.Mntc.Qry.Settings;

public record SettingsDto(
    IdGlobalOptions Settings, 
    IdGlobalSetupOptions_CUSTOMER CustomerSettings);