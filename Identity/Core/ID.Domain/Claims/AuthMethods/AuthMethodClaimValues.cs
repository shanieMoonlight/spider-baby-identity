namespace ID.Domain.Claims.AuthMethods;


/// <summary>
/// Actual string values for authentication method claims.
/// </summary>
public class AuthMethodClaimValues
{
    /// <summary>
    /// Standard password authentication
    /// </summary>
    public const string PASSWORD = "pwd";

    /// <summary>
    /// Multi-factor authentication
    /// </summary>
    public const string MULTI_FACTOR = $"mfa";

    /// <summary>
    /// One-time passcode (email/SMS)
    /// </summary>
    public const string OTP = "otp";

    /// <summary>
    /// Windows Integrated Authentication
    /// </summary>
    public const string WINDOWS_INTEGRATED_AUTH = "wia";

    /// <summary>
    /// RSA key / self-signed JWT (e.g., Microsoft Authenticator)
    /// </summary>
    public const string RSA = "rsa";

    /// <summary>
    /// Federated auth (JWT/SAML assertions)
    /// </summary>
    public const string FEDERATED = "fed";

    /// <summary>
    /// Facial recognition biometric
    /// </summary>
    public const string FACE = "face";

    /// <summary>
    /// Fingerprint biometric
    /// </summary>
    public const string FINGERPRINT = "fpt";

    /// <summary>
    /// Hardware-secured key (proof-of-possession)
    /// </summary>
    public const string HARDWARE_KEY = "hwk";

    /// <summary>
    /// <summary>Oauth Provider (Google, Facebook, etc.)</summary>
    /// </summary>
    public const string OAUTH = "oauth";



    /// <summary>
    /// <summary>Authentication using knowledge-based answers (security questions).</summary>
    /// </summary>
    public const string KBA = "kba";
}//Cls