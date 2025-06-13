//using MassTransit;

namespace ID.Application.AppAbs.MFA.AuthenticatorApps;

public class AuthAppSetupDto(
    string twoFactorSetupKey,
    string qrCodeImageData,
    string customerSecretKey,
    string account)
{
    /// <summary>
    ///  We need a unique PER USER key to identify this Setup
    ///  Must be saved: you need this value later to verify a validation code
    /// </summary>
    public string CustomerSecretKey { get; init; } = customerSecretKey;

    /// <summary>
    /// a string key for set up without QR code
    /// </summary>
    public string TwoFactorSetupKey { get; init; } = twoFactorSetupKey;

    /// <summary>
    /// a base64 formatted string that can be directly assigned to an img src
    /// </summary>
    public string QrCodeImageData { get; init; } = qrCodeImageData;

    /// <summary>
    /// a base64 formatted string that can be directly assigned to an img src
    /// </summary>
    public string Account { get; init; } = account;
}
