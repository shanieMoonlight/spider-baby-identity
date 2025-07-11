using ID.GlobalSettings.Constants;
using ID.GlobalSettings.Setup.Defaults;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ID.Infrastructure.Auth.JWT.Setup;

public class JwtOptions
{

    private int _tokenExpirationMinutes = IdGlobalDefaultValues.TOKEN_EXPIRATION_MINUTES;
    /// <summary>
    /// How long the token is valid for in minutes.
    /// </summary>
    public int TokenExpirationMinutes
    {
        get => _tokenExpirationMinutes;
        set => _tokenExpirationMinutes = value < 0
            ? IdGlobalDefaultValues.TOKEN_EXPIRATION_MINUTES
            : value;
    }

    /// <summary>
    /// Symmetric key for signing tokens.
    /// </summary>
    public string SymmetricTokenSigningKey { get; set; } = string.Empty;


    private string _tokenIssuer = IdGlobalDefaultValues.TOKEN_ISSUER;
    /// <summary>
    /// Token issuer identifier.
    /// </summary>
    public string TokenIssuer
    {
        get => _tokenIssuer;
        set => _tokenIssuer = string.IsNullOrWhiteSpace(value)
            ? IdGlobalDefaultValues.TOKEN_ISSUER
            : value;
    }

    private string _securityAlgorithm = IdGlobalDefaultValues.SECURITY_ALGORITHM;
    /// <summary>
    /// Security algorithm for symmetric encryption.
    /// </summary>
    public string SecurityAlgorithm
    {
        get => _securityAlgorithm;
        set => _securityAlgorithm = string.IsNullOrWhiteSpace(value)
            ? IdGlobalDefaultValues.SECURITY_ALGORITHM
            : value;
    }

    /// <summary>
    /// Current asymmetric key pair for signing tokens. Will be used for Jwt Signing and Validation.
    /// </summary>

    public AsymmetricPemKeyPair? CurrentAsymmetricKeyPair { get; set; }

    /// <summary>
    /// Gets or sets the collection of legacy asymmetric key pairs.
    /// <para></para>
    /// This collection is used to support older tokens that may have been signed with previous key pairs. (Key Rotation)
    /// </summary>
    /// use.</remarks>

    public List<AsymmetricPemKeyPair> LegacyAsymmetricKeyPairs { get; set; } = [];


    private string _asymmetricAlgorithm = IdGlobalDefaultValues.ASYMETRIC_ALGORITHM;
    /// <summary>
    /// Algorithm for asymmetric encryption.
    /// </summary>
    public string AsymmetricAlgorithm { 
        get => _asymmetricAlgorithm; 
        set => _asymmetricAlgorithm = string.IsNullOrWhiteSpace(value)
            ? IdGlobalDefaultValues.ASYMETRIC_ALGORITHM
            : value;
    }


    /// <summary>
    /// Policy for determining when refresh tokens should be updated during refresh operations.
    /// <para></para>
    /// Default is ThreeQuarterLife for a good balance of security and user experience.
    /// </summary>
    public RefreshTokenUpdatePolicy RefreshTokenUpdatePolicy { get; set; } = RefreshTokenUpdatePolicy.ThreeQuarterLife;



    private TimeSpan _refreshTokenTimeSpan = IdGlobalDefaultValues.REFRESH_TOKEN_EXPIRE_TIME_SPAN;
    /// <summary>
    /// Whether to use the RefreshTokens alongside the JWT Tokens.
    /// This will result in extra Database calls to check the tokens.
    /// You should probably reduce the expiration time of the JWT tokens if you are using this.
    /// <para></para>
    /// Default = <inheritdoc cref="IdGlobalDefaultValues.REFRESH_TOKEN_EXPIRE_TIME_SPAN"/>
    /// </summary>
    public TimeSpan RefreshTokenTimeSpan
    {
        get => _refreshTokenTimeSpan;
        set => _refreshTokenTimeSpan = value <= TimeSpan.Zero
            ? IdGlobalDefaultValues.REFRESH_TOKEN_EXPIRE_TIME_SPAN
            : value;
    }


    //- - - - - - - - - - - - - - - - - - //


    // Computed properties
    public SymmetricSecurityKey SecurityKey =>
        new(Encoding.UTF8.GetBytes(SymmetricTokenSigningKey));


    public static string TokenHeaderKey =>
        IdGlobalConstants.Authentication.TOKEN_HEADER_KEY;

    /// <summary>
    /// Whether to use asymmetric (RS256) instead of symmetric (HS256) encryption.
    /// </summary>
    public bool UseAsymmetricCrypto =>
        string.IsNullOrWhiteSpace(SymmetricTokenSigningKey);

}//Cls