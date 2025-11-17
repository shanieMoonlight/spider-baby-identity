using System.Text.Json.Serialization;

namespace ID.OAuth.Facebook.Data;

///// <summary>
///// Represents verified user data from Facebook OAuth after server-side token validation.
///// All data in this class comes from Facebook's servers and is trusted.
///// </summary>
//public class FacebookVerifiedPayload
//{
//    /// <summary>
//    /// Facebook user ID
//    /// </summary>
//    public string Id { get; set; } = string.Empty;

//    /// <summary>
//    /// User's email address (verified by Facebook)
//    /// </summary>
//    public string Email { get; set; } = string.Empty;

//    /// <summary>
//    /// User's full name
//    /// </summary>
//    public string Name { get; set; } = string.Empty;

//    /// <summary>
//    /// User's first name
//    /// </summary>
//    public string FirstName { get; set; } = string.Empty;

//    /// <summary>
//    /// User's last name
//    /// </summary>
//    public string LastName { get; set; } = string.Empty;

//    /// <summary>
//    /// URL to user's profile picture
//    /// </summary>
//    public string Picture { get; set; } = string.Empty;

//    /// <summary>
//    /// Whether the email is verified by Facebook
//    /// </summary>
//    public bool EmailVerified { get; set; }

//    /// <summary>
//    /// User's gender (if provided)
//    /// </summary>
//    public string? Gender { get; set; }

//    /// <summary>
//    /// User's locale (language/region)
//    /// </summary>
//    public string? Locale { get; set; }

//    /// <summary>
//    /// User's timezone offset from UTC
//    /// </summary>
//    public int? Timezone { get; set; }

//    /// <summary>
//    /// User's birthday (if provided and permitted by privacy settings)
//    /// </summary>
//    public DateTime? Birthday { get; set; }
//}

//----------------------//

///// <summary>
///// Internal model for Facebook's debug_token API response
///// </summary>
//internal class FacebookDebugResponse
//{
//    [JsonPropertyName("data")]
//    public FacebookDebugData? Data { get; set; }
//}

////----------------------//

///// <summary>
///// Internal model for Facebook token debug data
///// </summary>
//public class FacebookDebugData
//{
//    [JsonPropertyName("app_id")]
//    public string? AppId { get; set; }

//    [JsonPropertyName("type")]
//    public string? Type { get; set; }

//    [JsonPropertyName("application")]
//    public string? Application { get; set; }

//    [JsonPropertyName("data_access_expires_at")]
//    public long? DataAccessExpiresAt { get; set; }

//    [JsonPropertyName("expires_at")]
//    public long? ExpiresAt { get; set; }

//    [JsonPropertyName("is_valid")]
//    public bool IsValid { get; set; }

//    [JsonPropertyName("user_id")]
//    public string? UserId { get; set; }

//    [JsonPropertyName("scopes")]
//    public string[]? Scopes { get; set; }
//}

//----------------------//


//----------------------//

/// <summary>
/// Internal model for token validation result
/// </summary>
internal class FacebookTokenValidation
{
    public bool IsValid { get; set; }
    public long? ExpiresAt { get; set; }
    public string? UserId { get; set; }
    public string[]? Scopes { get; set; }
}
