using System.Text;
using System.Text.Json.Serialization;

namespace ID.OAuth.Facebook.Data;

public sealed class FacebookDebugTokenResponse
{
    [JsonPropertyName("data")]
    public FacebookDebugTokenData? Data { get; set; }

    //------------------------//

    public override string ToString() =>
        new StringBuilder()
          .AppendLine($"Data:")
          .AppendLine($"{Data}")
          .ToString();

}


//###########################################//

public sealed class FacebookDebugTokenData
{
    [JsonPropertyName("app_id")]
    public string? AppId { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("application")]
    public string? Application { get; set; }

    [JsonPropertyName("is_valid")]
    public bool IsValid { get; set; }

    [JsonPropertyName("user_id")]
    public string? UserId { get; set; }

    [JsonPropertyName("expires_at")]
    public DateTimeOffset? ExpiresAt { get; set; }

    [JsonPropertyName("data_access_expires_at")]
    public DateTimeOffset? DataAccessExpiresAt { get; set; }

    [JsonPropertyName("scopes")]
    public string[]? Scopes { get; set; }

    //------------------------//

    public override string ToString() =>
        new StringBuilder()
            .AppendLine($"AppId: {AppId ?? string.Empty}")
            .AppendLine($"Type: {Type ?? string.Empty}")
            .AppendLine($"Application: {Application ?? string.Empty}")
            .AppendLine($"IsValid: {IsValid}")
            .AppendLine($"UserId: {UserId ?? string.Empty}")
            .AppendLine($"ExpiresAt: {(ExpiresAt.HasValue ? ExpiresAt.Value.ToString("o") : string.Empty)}")
            .AppendLine($"DataAccessExpiresAt: {(DataAccessExpiresAt.HasValue ? DataAccessExpiresAt.Value.ToString("o") : string.Empty)}")
            .AppendLine($"Scopes: {(Scopes != null && Scopes.Length > 0 ? string.Join(", ", Scopes) : string.Empty)}")
            .ToString();
}//Cls