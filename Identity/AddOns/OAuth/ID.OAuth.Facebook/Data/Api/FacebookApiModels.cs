using System.Text.Json.Serialization;

namespace ID.OAuth.Facebook.Data.Api;

/// <summary>
/// Response from Facebook's debug_token endpoint for token validation.
/// </summary>
internal class FacebookDebugResponse
{
    [JsonPropertyName("data")]
    public FacebookTokenData? Data { get; set; }
}

//----------------------//

/// <summary>
/// Token validation data from Facebook's debug endpoint.
/// </summary>
internal class FacebookTokenData
{
    [JsonPropertyName("app_id")]
    public string? AppId { get; set; }

    [JsonPropertyName("is_valid")]
    public bool IsValid { get; set; }

    [JsonPropertyName("user_id")]
    public string? UserId { get; set; }

    [JsonPropertyName("expires_at")]
    public long? ExpiresAt { get; set; }

    [JsonPropertyName("scopes")]
    public string[]? Scopes { get; set; }
}

//----------------------//

/// <summary>
/// User profile response from Facebook's Graph API.
/// </summary>
internal class FacebookUserProfile
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("first_name")]
    public string? FirstName { get; set; }

    [JsonPropertyName("last_name")]
    public string? LastName { get; set; }

    [JsonPropertyName("picture")]
    public FacebookPictureData? Picture { get; set; }

    [JsonPropertyName("gender")]
    public string? Gender { get; set; }

    [JsonPropertyName("locale")]
    public string? Locale { get; set; }

    [JsonPropertyName("timezone")]
    public int? Timezone { get; set; }

    [JsonPropertyName("birthday")]
    public string? Birthday { get; set; }
}

//----------------------//

/// <summary>
/// Facebook profile picture data structure.
/// </summary>
internal class FacebookPictureData
{
    [JsonPropertyName("data")]
    public FacebookPictureInfo? Data { get; set; }
}

//----------------------//

/// <summary>
/// Facebook profile picture information.
/// </summary>
internal class FacebookPictureInfo
{
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("width")]
    public int Width { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }
}
