using System.Text.Json.Serialization;

namespace ID.OAuth.Amazon.Data;

public class AmazonTokenInfo
{
    [JsonPropertyName("client_id")] public string? ClientId { get; set; }
    [JsonPropertyName("expires_in")] public int? ExpiresIn { get; set; }
    [JsonPropertyName("scope")] public string? Scope { get; set; }
    [JsonPropertyName("user_id")] public string? UserId { get; set; }

    // Computed
    public DateTimeOffset? ExpiresAt { get; set; }
}
