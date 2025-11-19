using System.Text.Json.Serialization;

namespace ID.OAuth.Amazon.Data;

public class AmazonUserProfile
{
    [JsonPropertyName("user_id")] public string? UserId { get; set; }
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("email")] public string? Email { get; set; }
    [JsonPropertyName("postal_code")] public string? PostalCode { get; set; }
}
