using System.Text.Json.Serialization;

namespace ID.OAuth.Amazon.Data;

public class AmazonTokenInfo
{
    [JsonPropertyName("aud")] public string? ClientId { get; set; }
    [JsonPropertyName("exp")] public int? ExpiresIn { get; set; }
    [JsonPropertyName("scope")] public string? Scope { get; set; }
    [JsonPropertyName("iss")] public string? Issuer { get; set; }
    [JsonPropertyName("user_id")] public string? UserId { get; set; }
    [JsonPropertyName("app_id")] public string? AppId { get; set; }
    [JsonPropertyName("iat")] public DateTimeOffset? IssuedAt { get; set; }

    // Computed
    public DateTimeOffset? ExpiresAt { get => ExpiresIn.HasValue ? DateTimeOffset.UtcNow.AddSeconds(ExpiresIn.Value) : null; }
}
