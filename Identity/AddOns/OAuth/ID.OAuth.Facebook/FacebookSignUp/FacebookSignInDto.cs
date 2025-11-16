namespace ID.OAuth.Facebook.FacebookSignUp;

/// <summary>
/// Data transfer object for Facebook OAuth sign-in requests.
/// Contains only essential data - all identity claims come from server-side token verification.
/// </summary>
public sealed class FacebookSignInDto
{
    /// <summary>
    /// Facebook access token obtained from client-side OAuth flow.
    /// This token will be verified server-side against Facebook's API.
    /// </summary>
    public string FacebookAccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Optional subscription ID from your business system.
    /// This represents a server-generated subscription that the user is signing up for.
    /// </summary>
    public Guid? SubscriptionId { get; set; }

    /// <summary>
    /// Optional device identifier for tracking and security purposes.
    /// Used for device-specific policies and audit logging.
    /// </summary>
    public string? DeviceId { get; set; }

    // ✅ SECURITY NOTE: No identity claims (email, name, etc.) are accepted from the client.
    // All user identity data comes from Facebook's verified API response only.
}
