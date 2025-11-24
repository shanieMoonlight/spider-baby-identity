using System.ComponentModel;

namespace ID.Domain.Claims.AuthMethods;

/// <summary>
/// Strongly-typed claim value types for authentication methods.
/// Use these to ensure only allowed types are passed around the application.
/// </summary>
public enum AuthMethodRef
{
    /// <summary>Standard password authentication</summary>
    [Description("Password")]
    pwd = 1,

    /// <summary>One-time passcode (email/SMS)</summary>
    [Description("One-time passcode (email/SMS)")]
    otp = 2,

    /// <summary>Multi-factor authentication</summary>
    [Description("Multi-factor authentication")]
    mfa = 3,

    /// <summary>Windows Integrated Authentication</summary>
    [Description("Windows Integrated Authentication")]
    windowsIntegratedAuth = 4,

    /// <summary>RSA key / self-signed JWT (e.g., Microsoft Authenticator)</summary>
    [Description("RSA key / self-signed JWT")]
    rsa = 5,

    /// <summary>Federated auth (JWT/SAML assertions)</summary>
    [Description("Federated auth (JWT/SAML assertions)")]
    fed = 6,

    /// <summary>Facial recognition biometric</summary>
    [Description("Facial recognition biometric")]
    face = 7,

    /// <summary>Fingerprint biometric</summary>
    [Description("Fingerprint biometric")]
    fingerprint = 8,

    /// <summary>Hardware-secured key (proof-of-possession)</summary>
    [Description("Hardware-secured key (proof-of-possession)")]
    hwk = 9,

    /// <summary>Oauth Provider (Google, Facebook, etc.)</summary>
    [Description("Oauth Provider (Google, Facebook, etc.)")]
    oauth = 10,

    /// <summary>Authentication using knowledge-based answers (security questions).</summary>
    [Description("Authentication using knowledge-based answers (security questions).")]
    kba = 11
}

public static class AuthMethodClaimValueExtensions
{
    /// <summary>
    /// Convert a strongly-typed AuthMethodClaimValue to the string value used in claims.
    /// </summary>
    public static string ToClaimValue(this AuthMethodRef t) => t switch
    {
        AuthMethodRef.pwd => AuthMethodClaimValues.PASSWORD,
        AuthMethodRef.otp => AuthMethodClaimValues.OTP,
        AuthMethodRef.mfa => AuthMethodClaimValues.MULTI_FACTOR,
        AuthMethodRef.windowsIntegratedAuth => AuthMethodClaimValues.WINDOWS_INTEGRATED_AUTH,
        AuthMethodRef.rsa => AuthMethodClaimValues.RSA,
        AuthMethodRef.fed => AuthMethodClaimValues.FEDERATED,
        AuthMethodRef.face => AuthMethodClaimValues.FACE,
        AuthMethodRef.fingerprint => AuthMethodClaimValues.FINGERPRINT,
        AuthMethodRef.hwk => AuthMethodClaimValues.HARDWARE_KEY,
        AuthMethodRef.oauth => AuthMethodClaimValues.OAUTH,
        AuthMethodRef.kba => AuthMethodClaimValues.KBA,
        _ => throw new ArgumentOutOfRangeException(nameof(t), t, null)
    };

    /// <summary>
    /// Try to parse a claim string value into a MyClaimValueType.
    /// Returns false if the input doesn't match a known claim value.
    /// </summary>
    public static bool TryParseClaimValue(string? s, out AuthMethodRef value)
    {
        if (s is null)
        {
            value = default;
            return false;
        }

        switch (s)
        {
            case AuthMethodClaimValues.PASSWORD:
                value = AuthMethodRef.pwd;
                return true;
            case AuthMethodClaimValues.OTP:
                value = AuthMethodRef.otp;
                return true;
            case AuthMethodClaimValues.MULTI_FACTOR:
                value = AuthMethodRef.mfa;
                return true;
            case AuthMethodClaimValues.WINDOWS_INTEGRATED_AUTH:
                value = AuthMethodRef.windowsIntegratedAuth;
                return true;
            case AuthMethodClaimValues.RSA:
                value = AuthMethodRef.rsa;
                return true;
            case AuthMethodClaimValues.FEDERATED:
                value = AuthMethodRef.fed;
                return true;
            case AuthMethodClaimValues.FACE:
                value = AuthMethodRef.face;
                return true;
            case AuthMethodClaimValues.FINGERPRINT:
                value = AuthMethodRef.fingerprint;
                return true;
            case AuthMethodClaimValues.HARDWARE_KEY:
                value = AuthMethodRef.hwk;
                return true;
            case AuthMethodClaimValues.OAUTH:
                value = AuthMethodRef.oauth;
                return true;
            case AuthMethodClaimValues.KBA:
                value = AuthMethodRef.kba;
                return true;
            default:
                value = default; return false;
        }
    }
}
