using ID.GlobalSettings.Setup.Defaults;
using ID.GlobalSettings.Utility;
using ID.Infrastructure.Auth.Cookies;
using ID.Infrastructure.Auth.JWT.Setup;
using ID.Infrastructure.Persistance.EF.Setup.Options;
using ID.Infrastructure.Setup.Options;
using ID.Infrastructure.Setup.Passwords;
using ID.Infrastructure.Setup.SignIn;

namespace ID.Infrastructure.Setup;

public class IdInfrastructureSetupOptions
{
    /// <summary>
    /// Gets or sets the <see cref="IdPasswordOptions"/> for the identity system.
    /// <para></para>
    /// Default = <inheritdoc cref="InfrastructureDefaultValues.PASSWORD_OPTIONS"/>
    /// </summary>
    /// <value>
    /// The <see cref="IdPasswordOptions"/> for the identity system.
    /// </value>
    public required IdPasswordOptions? PasswordOptions { get; set; }


    /// <summary>
    /// Gets or sets the <see cref="IdSignInOptions"/> for the identity system.
    /// <para>
    /// </para>
    /// Default = <inheritdoc cref="InfrastructureDefaultValues.SIGN_IN_OPTIONS"/>
    /// </summary>
    /// <value>
    /// The <see cref="IdSignInOptions"/> for the identity system.
    /// </value>
    public required IdSignInOptions? SignInOptions { get; set; }

    //- - - - - - - - - - - - - -//

    /// <summary>
    /// Asymmetric public key in xml format
    /// </summary>
    public required string? AsymmetricTokenPublicKey_Xml { get; set; }

    /// <summary>
    /// Asymmetric private key in xml format
    /// </summary>
    public required string? AsymmetricTokenPrivateKey_Xml { get; set; }

    /// <summary>
    /// Asymmetric public key in PEM format
    /// </summary>
    public required string? AsymmetricTokenPublicKey_Pem { get; set; }

    /// <summary>
    /// Asymmetric private key in PEM format
    /// </summary>
    public required string? AsymmetricTokenPrivateKey_Pem { get; set; }

    /// <summary>
    /// Algorithm for asymmetric encryption.
    /// <para></para>
    /// Default = <inheritdoc cref="IdGlobalDefaultValues.ASYMETRIC_ALGORITHM"/>
    /// </summary>
    public required string? AsymmetricAlgorithm { get; set; } = IdGlobalDefaultValues.ASYMETRIC_ALGORITHM;

    /// <summary>
    /// The algorithm used to sign the token
    /// <para></para>
    /// Default = <inheritdoc cref="IdGlobalDefaultValues.SECURITY_ALGORITHM"/>
    /// </summary>
    public required string? SecurityAlgorithm { get; set; } = IdGlobalDefaultValues.SECURITY_ALGORITHM;


    //- - - - - - - - - - - - - -//


    /// <summary>
    /// How long the token is valid for.
    /// </summary>
    /// <para></para>
    /// Default = <inheritdoc cref="IdGlobalDefaultValues.TOKEN_EXPIRATION_MINUTES"/>
    public required int TokenExpirationMinutes { get; set; } = IdGlobalDefaultValues.TOKEN_EXPIRATION_MINUTES;

    /// <summary>
    /// Key for verifying token signature. SYMMETRIC
    /// MIN SIZE: 50 chars
    /// <para></para>
    /// Will default to Asymmetric if not set.
    /// </summary>
    public required string? SymmetricTokenSigningKey { get; set; }

    /// <summary>
    /// Writer of the token so we know it's from the IdentityServer
    /// <para></para>
    /// Default = <inheritdoc cref="IdGlobalDefaultValues.TOKEN_ISSUER"/>
    /// </summary>
    public required string? TokenIssuer { get; set; } = IdGlobalDefaultValues.TOKEN_ISSUER;

    /// <summary>
    /// <inheritdoc cref="JwtOptions.RefreshTokenUpdatePolicy"/>
    /// </summary>
    public required RefreshTokenUpdatePolicy? RefreshTokenUpdatePolicy { get; set; } = Auth.JWT.Setup.RefreshTokenUpdatePolicy.ThreeQuarterLife;

    /// <summary>
    /// <inheritdoc cref="JwtOptions.RefreshTokenTimeSpan"/>
    /// </summary>
    public required TimeSpan RefreshTokenTimeSpan { get; set; } = IdGlobalDefaultValues.REFRESH_TOKEN_EXPIRE_TIME_SPAN;


    //- - - - - - - - - - - - - -//

    /// <summary>
    /// The path to the login page.
    /// <para></para>
    /// Default = <inheritdoc cref="CookieDefaultValues.LOGIN_PATH"/>
    /// </summary>
    public required string? CookieLoginPath { get; set; } = CookieDefaultValues.LOGIN_PATH;

    /// <summary>
    /// The path to the logout page.
    /// <para></para>
    /// Default = <inheritdoc cref="CookieDefaultValues.LOGOUT_PATH"/>
    /// </summary>
    public required string? CookieLogoutPath { get; set; } = CookieDefaultValues.LOGOUT_PATH;

    /// <summary>
    /// The path to the access denied page.
    /// <para></para>
    /// Default = <inheritdoc cref="CookieDefaultValues.ACCESS_DENIED_PATH"/>
    /// </summary>
    public required string? CookieAccessDeniedPath { get; set; } = CookieDefaultValues.ACCESS_DENIED_PATH;

    /// <summary>
    /// Indicates whether the cookie expiration time should be reset on each request.
    /// <para></para>
    /// Default = <inheritdoc cref="CookieDefaultValues.SLIDING_EXPIRATION"/>
    /// </summary>
    public required bool? CookieSlidingExpiration { get; set; } = CookieDefaultValues.SLIDING_EXPIRATION;

    /// <summary>
    /// The cookie expiration time.
    /// <para></para>
    /// Default = <inheritdoc cref="CookieDefaultValues.EXPIRE_TIME_SPAN"/>
    /// </summary>
    public required TimeSpan? CookieExpireTimeSpan { get; set; } = CookieDefaultValues.EXPIRE_TIME_SPAN;

    //- - - - - - - - - - - - - -//

    /// <summary>
    /// Relative address of the Swagger page
    /// <para></para>
    /// Default = <inheritdoc cref="IdGlobalDefaultValues.SWAGGER_URL"/>
    /// </summary>
    public required string? SwaggerUrl { get; set; }

    /// <summary>
    /// List of external pages that we may need a token/cookie.
    /// <para></para>
    /// Default = []
    /// </summary>
    public required IEnumerable<string> ExternalPages { get; set; } = [];

    /// <summary>
    /// Allow user to access external pages when developing.
    /// <para></para>
    /// Default = <inheritdoc cref="InfrastructureDefaultValues.ALLOW_EXTERNAL_PAGES_DEV_MODE_ACCESS"/>
    /// </summary>
    public required bool? AllowExternalPagesDevModeAccess { get; set; } = InfrastructureDefaultValues.ALLOW_EXTERNAL_PAGES_DEV_MODE_ACCESS;

    /// <summary>
    /// Whether to use the Databse to store retrieve Tokens rather than the usual Token Providers.
    /// (Used for updating paswords and emails, etc.)
    /// Only use if absolutely necesarry.
    /// <para></para>
    /// Default = false
    /// </summary>
    public required bool? UseDbTokenProvider { get; set; } = InfrastructureDefaultValues.USE_DB_TOKEN_PROVIDER;

    //- - - - - - - - - - - - - -//

    /// <summary>
    /// How to contact the Identity Database
    /// </summary>
    public required string? ConnectionString { get; set; }


}//Cls


