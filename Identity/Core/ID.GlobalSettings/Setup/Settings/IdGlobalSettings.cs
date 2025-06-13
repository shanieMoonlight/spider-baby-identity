using ID.GlobalSettings.Setup.Defaults;
using ID.GlobalSettings.Setup.Options;
using StringHelpers;

namespace ID.GlobalSettings.Setup.Settings;

/// <summary>
/// Provides centralized management of global application settings, 
/// including defaults, validation, and configuration via <see cref="IdGlobalOptions"/>.
/// </summary>
internal partial class IdGlobalSettings
{

    private static string _claimTypePrefix = IdGlobalDefaultValues.CLAIM_TYPE_PREFIX;
    /// <inheritdoc cref="IdGlobalOptions.ClaimTypePrefix"/>    
    internal static string ClaimTypePrefix
    {
        get => _claimTypePrefix.IsNullOrWhiteSpace() ? IdGlobalDefaultValues.CLAIM_TYPE_PREFIX : _claimTypePrefix;
        set => _claimTypePrefix = value;
    }

    //------------------------------------//

    internal static void Setup(IdGlobalOptions options)
    {
        ClaimTypePrefix = options.ClaimTypePrefix;
    }





}//Cls
