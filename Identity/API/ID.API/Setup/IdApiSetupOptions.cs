using ID.Application.Setup;
using ID.GlobalSettings.Setup.Defaults;
using ID.GlobalSettings.Setup.Options;
using ID.GlobalSettings.Utility;
using ID.Infrastructure.Auth.JWT.Setup;
using ID.Infrastructure.Setup;
using ID.Infrastructure.Setup.Options;
using ID.Infrastructure.Setup.Passwords;
using ID.Infrastructure.Setup.SignIn;
using ID.IntegrationEvents.Setup;

namespace ID.API.Setup;

public class IdApiSetupOptions
{

    //#########################################################//
    //###################      Global       ###################//
    //#########################################################//

    #region Global

    /// <summary>
    /// <inheritdoc cref="IdGlobalOptions.ApplicationName"/>
    /// </summary>
    public string ApplicationName { get; set; } = string.Empty;


    /// <summary>
    /// The url that locates the Customer accounts section of your site.
    /// Will be used for links in emails (ForgotPwd, Registration etc)
    /// </summary>
    public string MntcAccountsUrl { get; set; } = string.Empty;


    /// <summary>
    /// <inheritdoc cref="IdGlobalOptions.MntcTeamMinPosition"/>
    /// </summary>
    public int MntcTeamMinPosition { get; set; } = IdGlobalDefaultValues.MIN_TEAM_POSITION;


    /// <summary>
    /// <inheritdoc cref="IdGlobalOptions.MntcTeamMaxPosition"/>
    /// </summary>
    public int MntcTeamMaxPosition { get; set; } = IdGlobalDefaultValues.MAX_TEAM_POSITION;


    /// <summary>
    /// <inheritdoc cref="IdGlobalOptions.MntcTeamMaxPosition"/>
    /// </summary>
    public int SuperTeamMinPosition { get; set; } = IdGlobalDefaultValues.MIN_TEAM_POSITION;


    /// <summary>
    /// <inheritdoc cref="IdGlobalOptions.SuperTeamMaxPosition"/>
    /// </summary>
    public int SuperTeamMaxPosition { get; set; } = IdGlobalDefaultValues.MAX_TEAM_POSITION;


    /// <summary>
    /// <inheritdoc cref="IdGlobalOptions.ClaimTypePrefix"/>
    /// </summary>
    public static string ClaimTypePrefix { get; set; } = IdGlobalDefaultValues.CLAIM_TYPE_PREFIX;


    /// <summary>
    /// <inheritdoc cref="IdGlobalOptions.PhoneTokenTimeSpan"/>
    /// </summary>
    public TimeSpan PhoneTokenTimeSpan { get; set; } = IdGlobalDefaultValues.PHONE_TOKEN_EXPIRE_TIME_SPAN;



    /// <summary>
    /// <inheritdoc cref="IdGlobalOptions.JwtRefreshTokensEnabled"/>
    /// </summary>
    public bool JwtRefreshTokensEnabled { get; set; }


    #endregion


    //#########################################################//
    //###################    Application    ###################//
    //#########################################################//

    #region Application


    /// <summary>
    /// <inheritdoc cref="IdApplicationOptions.FromAppHeaderKey"/>
    /// </summary>
    public string? FromMobileAppHeaderKey { get; set; }

    /// <summary>
    /// <inheritdoc cref="IdApplicationOptions.FromAppHeaderValue"/>
    /// </summary>
    public string? FromMoblieAppHeaderValue { get; set; }


    #endregion

    //#########################################################//
    //###################   INFRASTRUCTURE   ###################//
    //#########################################################//

    #region Infrastructure


    /// <summary>
    /// <inheritdoc cref="IdInfrastructureSetupOptions.UseDbTokenProvider"/>
    /// </summary>
    public bool UseDbTokenProvider { get; set; } = false;

    /// <summary>
    /// How to contact the Identity Database
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    ///<inheritdoc cref="IdInfrastructureSetupOptions.CookieLoginPath"/>
    /// </summary>
    public string? CookieLoginPath { get; set; }

    /// <summary>
    ///<inheritdoc cref="IdInfrastructureSetupOptions.CookieLogoutPath"/>
    /// </summary>
    public string? CookieLogoutPath { get; set; }

    /// <summary>
    ///<inheritdoc cref="IdInfrastructureSetupOptions.CookieAccessDeniedPath"/>
    /// </summary>
    public string? CookieAccessDeniedPath { get; set; }

    /// <summary>
    ///<inheritdoc cref="IdInfrastructureSetupOptions.CookieSlidingExpiration"/>
    /// </summary>
    public bool? CookieSlidingExpiration { get; set; }

    /// <summary>
    ///<inheritdoc cref="IdInfrastructureSetupOptions.CookieExpireTimeSpan"/>
    /// </summary>
    public TimeSpan? CookieExpireTimeSpan { get; set; }

    /// <summary>
    ///<inheritdoc cref="IdInfrastructureSetupOptions.PasswordOptions"/>
    /// </summary>
    public IdPasswordOptions? PasswordOptions { get; set; }

    /// <summary>
    ///<inheritdoc cref="IdInfrastructureSetupOptions.SignInOptions"/>
    /// </summary>
    public IdSignInOptions? SignInOptions { get; set; }


    /// <summary>
    /// <inheritdoc cref="IdInfrastructureSetupOptions.AsymmetricPemKeyPair"/>
    /// </summary>
    public AsymmetricPemKeyPair? JwtAsymmetricPemKeyPair { get; set; }

    /// <summary>
    /// <inheritdoc cref="IdInfrastructureSetupOptions.LegacyAsymmetricPemKeyPairs"/>
    /// </summary>
    public IEnumerable<AsymmetricPemKeyPair> JwtLegacyAsymmetricPemKeyPairs { get; set; } = [];


    /// <summary>
    /// <inheritdoc cref="IdInfrastructureSetupOptions.AsymmetricXmlKeyPair"/>
    /// </summary>
    public AsymmetricXmlKeyPair? JwtAsymmetricXmlKeyPair { get; set; }

    /// <summary>
    /// <inheritdoc cref="IdInfrastructureSetupOptions.LegacyAsymmetricXmlKeyPairs"/>
    /// </summary>
    public IEnumerable<AsymmetricXmlKeyPair> JwtLegacyAsymmetricXmlKeyPairs { get; set; } = [];


    /// <summary>
    /// <inheritdoc cref="IdInfrastructureSetupOptions.AsymmetricAlgorithm"/>
    /// </summary>
    public string JwtAsymmetricAlgorithm { get; set; } = IdGlobalDefaultValues.ASYMETRIC_ALGORITHM;


    /// <summary>
    /// <inheritdoc cref="IdInfrastructureSetupOptions.SecurityAlgorithm"/>
    /// </summary>
    public string JwtSecurityAlgorithm { get; set; } = IdGlobalDefaultValues.SECURITY_ALGORITHM;



    /// <summary>
    /// <inheritdoc cref="IdInfrastructureSetupOptions.TokenExpirationMinutes"/>
    /// </summary>
    public int JwtTokenExpirationMinutes { get; set; } = IdGlobalDefaultValues.TOKEN_EXPIRATION_MINUTES;

    /// <summary>
    /// <inheritdoc cref="IdInfrastructureSetupOptions.SymmetricTokenSigningKey"/>
    /// </summary>
    public string? JwtTokenSigningKey { get; set; }

    /// <summary>
    /// <inheritdoc cref="IdInfrastructureSetupOptions.TokenIssuer"/>
    /// </summary>
    public string JwtTokenIssuer { get; set; } = IdGlobalDefaultValues.TOKEN_ISSUER;

    /// <summary>
    /// <inheritdoc cref="IdInfrastructureSetupOptions.RefreshTokenUpdatePolicy"/>
    /// </summary>
    public RefreshTokenUpdatePolicy? JwtRefreshTokenUpdatePolicy { get; set; }


    /// <summary>
    /// <inheritdoc cref="IdInfrastructureSetupOptions.RefreshTokenTimeSpan"/>
    /// </summary>
    public TimeSpan JwtRefreshTokenTimeSpan { get; set; }

    /// <summary>
    /// <inheritdoc cref="IdInfrastructureSetupOptions.ExternalPages"/>
    /// </summary>
    public IEnumerable<string> ExternalPages { get; set; } = [];


    /// <summary>
    /// <inheritdoc cref="IdInfrastructureSetupOptions.SwaggerUrl"/>
    /// </summary>
    public string? SwaggerUrl { get; set; } = InfrastructureDefaultValues.SWAGGER_URL;

    /// <summary>
    /// <inheritdoc cref="IdInfrastructureSetupOptions.AllowExternalPagesDevModeAccess"/>
    /// </summary>
    public bool AllowExternalPagesDevModeAccess { get; set; } = InfrastructureDefaultValues.ALLOW_EXTERNAL_PAGES_DEV_MODE_ACCESS;


    #endregion


    //#########################################################//
    //################   INtegration Events   #################//
    //#########################################################//

    #region Infrastructure


    /// <summary>
    /// Set to true if your host application already uses MassTransit.
    /// When true, MyId will use a separate IMyIdMtBus to avoid conflicts.
    /// When false (default), MyId will use the standard IBus.
    /// </summary>
    public bool UseSeperateEventBus { get; set; } = false;


    #endregion

    //#########################################################//


    internal IdApplicationOptions GetApplicationSetupOptions() => new()
    {
        FromAppHeaderKey = FromMobileAppHeaderKey,
        FromAppHeaderValue = FromMoblieAppHeaderValue,
    };


    //------------------------------------//


    internal IdInfrastructureSetupOptions GetInfrastructureSetupOptions() => new()
    {
        ConnectionString = ConnectionString,
        AsymmetricAlgorithm = JwtAsymmetricAlgorithm,
        PasswordOptions = PasswordOptions,
        SignInOptions = SignInOptions,
        UseDbTokenProvider = UseDbTokenProvider,
        SecurityAlgorithm = JwtSecurityAlgorithm,
        TokenExpirationMinutes = JwtTokenExpirationMinutes,
        TokenIssuer = JwtTokenIssuer,
        SymmetricTokenSigningKey = JwtTokenSigningKey,
        ExternalPages = ExternalPages,
        AllowExternalPagesDevModeAccess = AllowExternalPagesDevModeAccess,
        SwaggerUrl = SwaggerUrl,
        CookieAccessDeniedPath = CookieAccessDeniedPath,
        CookieLoginPath = CookieLoginPath,
        CookieLogoutPath = CookieLogoutPath,
        CookieExpireTimeSpan = CookieExpireTimeSpan,
        CookieSlidingExpiration = CookieSlidingExpiration,
        RefreshTokenUpdatePolicy = JwtRefreshTokenUpdatePolicy,
        RefreshTokenTimeSpan = JwtRefreshTokenTimeSpan,
        AsymmetricPemKeyPair = JwtAsymmetricPemKeyPair,
        LegacyAsymmetricPemKeyPairs = JwtLegacyAsymmetricPemKeyPairs,
        AsymmetricXmlKeyPair = JwtAsymmetricXmlKeyPair,
        LegacyAsymmetricXmlKeyPairs = JwtLegacyAsymmetricXmlKeyPairs,
    };


    //------------------------------------//


    internal IdGlobalOptions GetGlobalSetupOptions() => new()
    {
        ApplicationName = ApplicationName,
        MntcTeamMaxPosition = MntcTeamMaxPosition,
        MntcTeamMinPosition = MntcTeamMinPosition,
        SuperTeamMinPosition = SuperTeamMinPosition,
        SuperTeamMaxPosition = SuperTeamMaxPosition,
        ClaimTypePrefix = ClaimTypePrefix,
        JwtRefreshTokensEnabled = JwtRefreshTokensEnabled,
        PhoneTokenTimeSpan = PhoneTokenTimeSpan,
        MntcAccountsUrl = MntcAccountsUrl,

    };

    //------------------------------------//

    internal IntegrationEventsOptions GetIntegrationEventsOptions() => new()
    {
        UseSeperateEventBus = UseSeperateEventBus,
    };


    //#########################################################//




}//Cls
