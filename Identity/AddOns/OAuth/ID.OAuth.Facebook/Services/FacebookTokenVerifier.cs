using System.Net.Http.Json;
using System.Text.Json.Serialization;
using ID.OAuth.Facebook.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ID.OAuth.Facebook.Services;

internal sealed partial class FacebookTokenVerifier(
    IHttpClientFactory httpFactory, 
    IOptions<IdOAuthFacebookOptions> opts, 
    ILogger<FacebookTokenVerifier> log) 
    : IFacebookTokenVerifier
{
    private readonly HttpClient _http = httpFactory.CreateClient("facebook-graph");
    private readonly IdOAuthFacebookOptions _opts = opts.Value;

    public async Task<FacebookTokenVerificationResult> VerifyAsync(string userAccessToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userAccessToken))
            return FacebookTokenVerificationResult.Invalid("empty_user_token");

        if (string.IsNullOrWhiteSpace(_opts.AppId) || string.IsNullOrWhiteSpace(_opts.AppSecret))
            return FacebookTokenVerificationResult.Invalid("missing_server_credentials");

        try
        {
            var appToken = $"{_opts.AppId}|{_opts.AppSecret}";
            var url = $"debug_token?input_token={Uri.EscapeDataString(userAccessToken)}&access_token={Uri.EscapeDataString(appToken)}";

            var resp = await _http.GetFromJsonAsync<DebugTokenResponse>(url, cancellationToken);
            if (resp?.Data is null)
                return FacebookTokenVerificationResult.Invalid("invalid_response");

            var d = resp.Data;

            if (!d.IsValid)
                return FacebookTokenVerificationResult.Invalid("token_not_valid");

            if (!string.Equals(d.AppId, _opts.AppId, StringComparison.Ordinal))
                return FacebookTokenVerificationResult.Invalid("app_id_mismatch");

            var expiresAt = d.ExpiresAt.HasValue ? DateTimeOffset.FromUnixTimeSeconds(d.ExpiresAt.Value) : (DateTimeOffset?)null;

            return FacebookTokenVerificationResult.Valid(d.UserId ?? string.Empty, expiresAt, d.Scopes ?? []);
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Failed to verify facebook token");
            return FacebookTokenVerificationResult.Invalid("exception");
        }
    }

    private sealed class DebugTokenResponse
    {
        [JsonPropertyName("data")]
        public DebugTokenData? Data { get; set; }
    }

    private sealed class DebugTokenData
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
}
