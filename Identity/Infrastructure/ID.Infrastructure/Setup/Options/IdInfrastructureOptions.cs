using ID.GlobalSettings.Utility;

namespace ID.Infrastructure.Setup.Options;

/// <summary>
/// Options for configuring the general infrastructure stuff.
/// this will be used to configure the password strength validators.
/// </summary>
public class IdInfrastructureOptions
{
    private  bool _useDbTokenProvider = InfrastructureDefaultValues.USE_DB_TOKEN_PROVIDER;
    /// <inheritdoc cref="IdInfrastructureSetupOptions.UseDbTokenProvider"/>
    internal  bool UseDbTokenProvider
    {
        get => _useDbTokenProvider;
        set => _useDbTokenProvider = value;
    }



    private  string? _swaggerUrl;
    /// <inheritdoc cref="IdInfrastructureSetupOptions.SwaggerUrl"/>
    internal  string? SwaggerUrl
    {
        get => _swaggerUrl;
        set => _swaggerUrl = string.IsNullOrWhiteSpace(value)
            ? InfrastructureDefaultValues.SWAGGER_URL
            : value;
    }


    private  List<string> _externalPages = [];
    /// <inheritdoc cref="IdInfrastructureSetupOptions.ExternalPages"/>
    internal  List<string> ExternalPages
    {
        get => _externalPages;
        set => _externalPages = value ?? [];
    }


    private  bool _allowExternalPagesDevModeAccess = InfrastructureDefaultValues.ALLOW_EXTERNAL_PAGES_DEV_MODE_ACCESS;
    /// <inheritdoc cref="IdInfrastructureSetupOptions.AllowExternalPagesDevModeAccess"/>
    internal  bool AllowExternalPagesDevModeAccess
    {
        get => _allowExternalPagesDevModeAccess;
        set => _allowExternalPagesDevModeAccess = value;
    }

}//Cls

