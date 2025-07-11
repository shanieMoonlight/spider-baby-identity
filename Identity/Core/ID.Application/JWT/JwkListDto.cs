using System.Text.Json.Serialization;

namespace ID.Application.JWT;

public class JwkDto
{
    [JsonPropertyName("kty")]
    public string Kty { get; set; } = string.Empty;
    [JsonPropertyName("use")]
    public string Use { get; set; } = string.Empty;
    [JsonPropertyName("alg")]
    public string Alg { get; set; } = string.Empty;
    [JsonPropertyName("n")]
    public string N { get; set; } = string.Empty;
    [JsonPropertyName("e")]
    public string E { get; set; } = string.Empty;
    [JsonPropertyName("kid")]
    public string Kid { get; set; } = string.Empty;
}

public class JwkListDto(List<JwkDto> keys)
{
    [JsonPropertyName("keys")]
    public List<JwkDto> Keys { get; set; } = keys;
}
