using Microsoft.IdentityModel.Tokens;

namespace ID.GlobalSettings.Setup.Defaults;

#pragma warning disable IDE1006 // Naming Styles
public partial class IdGlobalDefaultValues
{
    //Public so user of the lib can see it
    ///// <summary>
    ///// Writer of the token so we know it's from the MyIdentity
    ///// </summary>
    public const string TOKEN_ISSUER = "spiderbaby.myid";

    //Public so user of the lib can see it
    /// <summary>
    /// shanie.moonlight.myidentity.claims
    /// </summary>
    public const string CLAIM_TYPE_PREFIX = "myid";

    //Highest team position
    /// <summary>
    /// 3 (User, Mgr, Admin)
    /// </summary>
    internal const int MAX_TEAM_POSITION = 3;

    //Lowest team position, used in  creation and updates
    /// <summary>
    /// 1
    /// </summary>
    internal const int MIN_TEAM_POSITION = 1;

    /// <summary>
    /// false
    /// </summary>
    internal const bool REFRESH_TOKENS_ENABLED = false;

    /// <summary>
    /// TimeSpan.FromDays(7) - 1 week
    /// </summary>
    internal static readonly TimeSpan REFRESH_TOKEN_EXPIRE_TIME_SPAN = TimeSpan.FromDays(7);

    /// <summary>
    /// TimeSpan.FromHours(6)
    /// </summary>
    internal static readonly TimeSpan PHONE_TOKEN_EXPIRE_TIME_SPAN = TimeSpan.FromHours(6);

    //How long the cookie is valid for.
    /// <summary>
    /// 120
    /// </summary>
    public const int COOKIE_EXPIRATION_MINUTES = 120;

    // How long the token is valid for.
    /// <summary>
    /// 1440
    /// </summary>
    public const int TOKEN_EXPIRATION_MINUTES = 1440;

    // The algorithm used to sign the token
    /// <summary>
    /// SecurityAlgorithms.HmacSha256
    /// </summary>
    public const string SECURITY_ALGORITHM = SecurityAlgorithms.HmacSha256;


    // What algo to use when asymetrically encrypting JWTs
    /// <summary>
    /// SecurityAlgorithms.RsaSha256
    /// </summary>
    public const string ASYMETRIC_ALGORITHM = SecurityAlgorithms.RsaSha256;


    // Key for verifying token signature. SYMMETRIC
    /// <summary>
    /// 50
    /// </summary>
    public const int MIN_SYMMETRIC_SIGNING_KEY_LENGTH = 50;


}//Cls
#pragma warning restore IDE1006 // Naming Styles